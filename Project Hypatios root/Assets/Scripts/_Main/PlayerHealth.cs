using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;
using Kryz.CharacterStats;

public class PlayerHealth : MonoBehaviour
{

    [FoldoutGroup("Stats")] public BaseStatusEffectObject ailment_DamagedTissue;
    [FoldoutGroup("Stats")] public float chanceGetting_DamagedTissue = 0.1f;
    [FoldoutGroup("Stats")] public float damageThreshold_DamagedTissue = 51f;

    public CharacterStat maxHealth = new CharacterStat(100);
    public CharacterStat healthRegen = new CharacterStat(0);
    public int CachedHealCapsules = 0;
    public int limitCapsule = 5;
    public GameEvent OnPlayerDead;
    public float curHealth;
    public float targetHealth;
    public float HealthSpeed = 4;
    [FoldoutGroup("Statistics")] public BaseStatValue stat_damage_taken;
    [FoldoutGroup("Blur Effect")] public float dof_Max = 55f;
    [FoldoutGroup("Blur Effect")] public float dof_Power = 13f;
    [Range(0f, 100f)] public float alcoholMeter = 0;
    public Vector3 camRecoilDamage = new Vector3(3f, 3f, 3f);
    [InfoBox("Only for Elena")]
    [FoldoutGroup("Elena Section")] public CharacterStat armorRating = new CharacterStat(1);
    public CharacterStat digestion = new CharacterStat(1);
    float healthAfterHeal = 0f;
    public bool isDead;
    public SlowMotion slow;
    public GameObject deathCamera;

    //Die
    //public Volume postProcess;
    //private DepthOfField dof;
    public CharacterScript character;
    Animator anim;

    public GameManager gameManager;
    float timeAfterDeath = 0f;

    private DepthOfField dof;
    private Vignette vignette;
    private FloatParameter vignetteParam_Intensity;
    private ColorParameter vignetteParam_Color;

    private Color vignetteColor;

    // Start is called before the first frame update
    void Start()
    {
        //curHealth = maxHealth;
        //targetHealth = curHealth;

        {
            var dof_ = Hypatios.Game.postProcessVolume.profile.GetSetting<DepthOfField>();
            dof = dof_;
            dof.focalLength.value = 1f;
        }

        {
            var vignette_ = Hypatios.Game.postProcessVolume.profile.GetSetting<Vignette>();
            vignette = vignette_;

            {
                vignetteParam_Intensity = vignette.intensity;
                vignetteParam_Intensity.value = 0.35f;
            }

            {
                vignetteParam_Color = vignette.color;
                vignetteParam_Color.value = Color.black;
            }
        }

        //postProcess.sharedProfile.TryGet<DepthOfField>(out dof);
        anim = character.Anim;
        slow.ReturnTime();
    }

    private UnityEngine.UI.Slider sliderHealth;
    private UnityEngine.UI.Slider sliderHealthSpeed;

    void Update()
    {
        MainGameHUDScript.Instance.healthPoint.text = "" + Mathf.RoundToInt(curHealth);
        sliderHealth = MainGameHUDScript.Instance.healthSlider;
        sliderHealthSpeed = MainGameHUDScript.Instance.justDamagedHealthSlider;

        CapsuleHealing();

        curHealth = Mathf.MoveTowards(curHealth, targetHealth, HealthSpeed * Time.deltaTime);

        if (transform.position.y < -100)
        {
            Die();
            curHealth = 0;
            isDead = true;
        }

        if ((targetHealth / maxHealth.Value) < 0.2f)
        {
            //if (Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.Elena)
                Hypatios.Game.RuntimeTutorialHelp("Your health is low", "Your health is low, you need to find the green glowing capsule to heal yourself.", "20%LowHealth");
        }

        if (alcoholMeter >= 99f)
        {
            curHealth = -1f;
            targetHealth = -1f;
        }

        if (curHealth <= 0f)
        {
            Die();
     
        }
        else
        {
            float capPostFX_Health = 100;

            float limitDof = Mathf.Clamp(capPostFX_Health / curHealth, 1, 55);
            float vignetteIntensity = Mathf.Lerp(0.35f, 0.9f, 1 - curHealth / capPostFX_Health);
            float vignetteRed = Mathf.Lerp(0f, 1f, 1.1f - curHealth / capPostFX_Health);

            vignetteColor.r = vignetteRed;

            if (dof.focalLength.value > limitDof)
            {
                dof.focalLength.value -= Time.deltaTime * 2.7f;
            }

            vignetteParam_Intensity.value = vignetteIntensity;
            vignetteParam_Color.value = vignetteColor;

        }

        //regen
        if (isDead == false && targetHealth > 1f)
            targetHealth += Time.deltaTime * healthRegen.Value;

        if (targetHealth < curHealth)
        {
            sliderHealth.value = targetHealth;
            sliderHealthSpeed.value = curHealth;
        }
        else
        {
            sliderHealth.value = curHealth;
            sliderHealthSpeed.value = targetHealth;
        }

        sliderHealth.maxValue = maxHealth.Value;
        sliderHealthSpeed.maxValue = maxHealth.Value;

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    targetHealth -= 20;
        //}

        if (targetHealth <= -99f)
        {
            targetHealth = -99;
        }
        else if (targetHealth > maxHealth.Value)
        {
            targetHealth = maxHealth.Value;
        }

       
    }

    #region Capsule Heal

    public void AddCacheCapsule()
    {
        CachedHealCapsules++;
        CachedHealCapsules = Mathf.Clamp(CachedHealCapsules, 0, limitCapsule);
    }

    public void CapsuleHealing()
    {
        int additionalHeal = Mathf.Clamp(Mathf.RoundToInt(maxHealth.Value / 50f), 1, 99);
        int amountToHeal = 3 * additionalHeal;
        amountToHeal = Mathf.Clamp(amountToHeal, 10, 100);

        if (CachedHealCapsules > 0 && targetHealth + 0.1f < maxHealth.Value)
        {
            Heal(amountToHeal, instantHeal: true);
            CachedHealCapsules--;
        }

        CachedHealCapsules = Mathf.Clamp(CachedHealCapsules, 0, limitCapsule);


    }
    #endregion

    public void Heal(int healNum, float healthSpeed = 0, bool instantHeal = false)
    {
        if (healthSpeed > 0f)
        {
            HealthSpeed = (healthSpeed * digestion.Value);
        }

        if (instantHeal == false)
            targetHealth = Mathf.Clamp(targetHealth + healNum, 0f, maxHealth.Value);
        else 
        {
            curHealth = Mathf.Clamp(curHealth + healNum, 0f, maxHealth.Value);
            targetHealth = Mathf.Clamp(targetHealth + healNum, 0f, maxHealth.Value);
        }
        soundManagerScript.instance.PlayOneShot("reward");
    }
    public void takeDamage(int damage, float speed = 11, float shakinessFactor = 1)
    {
        if (character.isCheatMode)
        {
            return;
        }

        if (enabled == false)
        {
            return;
        }

        bool isHealing = false;

        if (healthAfterHeal > 0)
            healthAfterHeal -= damage;       

        Hypatios.Game.Add_PlayerStat(stat_damage_taken, damage);

        {
            if (targetHealth > curHealth)
                isHealing = true;

            if (isHealing)
                targetHealth = curHealth;

            targetHealth -= (damage / armorRating.Value);
        }

        if (dof.focalLength.value < dof_Max)
        {
            float blurPower = 100f / maxHealth.Value; //100/600 = x0.16
            dof.focalLength.value = damage * dof_Power * blurPower;

            if (dof.focalLength.value > dof_Max)
            {
                dof.focalLength.value = dof_Max;
            }
        }

        var recoilAmount = 1f * Mathf.Clamp(damage/10, 1, 10);
        var camRecoil = new Vector3(camRecoilDamage.x * recoilAmount, camRecoilDamage.y * recoilAmount, camRecoilDamage.z * recoilAmount);

        Hypatios.Player.Weapon.Recoil.CustomRecoil(camRecoil, shakinessFactor, Recoil.RecoilType.TakeDamage);

        float random = Random.Range(0f, 1f);

        if (random > 0.5f)
        {
            soundManagerScript.instance.PlayOneShot("hurt.0");
        }
        else
        {
            soundManagerScript.instance.PlayOneShot("hurt.1");
        }

        if (random < chanceGetting_DamagedTissue && damage > damageThreshold_DamagedTissue)
        {
            if (Hypatios.Player.IsStatusEffectGroup(ailment_DamagedTissue) == false)
            {
                ailment_DamagedTissue.AddStatusEffectPlayer(30f);
                DeadDialogue.PromptNotifyMessage_Mod("Aldrich's regeneration tissue has been damaged.", 4f);
            }
        }

        HealthSpeed = speed;
    }

    bool die = false;

    public void Die()
    {
        if (character.isLimitedIntroMode)
        {
            return;
        }
        if (ChamberLevelController.Instance.chamberObject.isBanDying)
        {
            curHealth = 1f;
            targetHealth = 1f;
            return;
        }

        curHealth = 0;
        isDead = true;

        //postProcess.sharedProfile.TryGet<DepthOfField>(out dof);
        slow.SlowMo();
        if (dof.focalLength.value < 150f)
        {
            dof.focalLength.value += Time.unscaledDeltaTime * 50f;
        }


        timeAfterDeath += Time.unscaledDeltaTime;
        if (timeAfterDeath > 5f)
        {  
            Debug.Log("isCalled");
            gameManager.RestartLevel();
        }

        if (!die)
        {

            if (character.tutorialMode == false) Hypatios.Game.PlayerDie();
            deathCamera.gameObject.SetActive(true);
            cameraScript.instance.gameObject.SetActive(false);
            Hypatios.Game.Death_NoSaturation();
            MainGameHUDScript.Instance.gameObject.SetActive(false);
            OnPlayerDead.Raise();

            soundManagerScript.instance.Play("Die");

            if (MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.StopMusic();
            }
        }

        die = true;
    }
}
