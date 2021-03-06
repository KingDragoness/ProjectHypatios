using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;

public class health : MonoBehaviour
{

    [SerializeField] public float maxHealth = 100f;
    public float curHealth;
    public float targetHealth;
    public float healPerSecond;
    public float healthRegen = 0f;
    public float HealthSpeed = 4;
    [InfoBox("Only for Elena")]
    [FoldoutGroup("Elena Section")] public float armorStrength = 1;
    float healthAfterHeal = 0f;
    public bool isDead;
    public SlowMotion slow;
    public GameObject deathCamera;

    //Die
    //public Volume postProcess;
    //private DepthOfField dof;
    public characterScript character;
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
            var dof_ = FPSMainScript.instance.postProcessVolume.profile.GetSetting<DepthOfField>();
            dof = dof_;
            dof.focalLength.value = 1f;
        }

        {
            var vignette_ = FPSMainScript.instance.postProcessVolume.profile.GetSetting<Vignette>();
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

        curHealth = Mathf.MoveTowards(curHealth, targetHealth, HealthSpeed * Time.deltaTime);

        if (transform.position.y < -100)
        {
            Die();
            curHealth = 0;
            isDead = true;
        }

        if ((targetHealth / maxHealth) < 0.2f)
        {
            if (FPSMainScript.instance.currentGamemode != FPSMainScript.CurrentGamemode.Elena)
                FPSMainScript.instance.RuntimeTutorialHelp("Your health is low", "Your health is low, you need to find the green glowing capsule to heal yourself.", "20%LowHealth");
        }

        if (curHealth <= 0f)
        {
            Die();
            curHealth = 0;
            isDead = true;
        }
        else
        {
            float capPostFX_Health = 100;

            float limitDof = Mathf.Clamp(capPostFX_Health / curHealth, 1, 48);
            float vignetteIntensity = Mathf.Lerp(0.35f, 0.7f, 1 - curHealth / capPostFX_Health);
            float vignetteRed = Mathf.Lerp(0f, 1f, 1 - curHealth / capPostFX_Health);

            vignetteColor.r = vignetteRed;

            if (dof.focalLength.value > limitDof)
            {
                dof.focalLength.value -= Time.deltaTime * 2.7f;
            }

            vignetteParam_Intensity.value = vignetteIntensity;
            vignetteParam_Color.value = vignetteColor;

        }

        //regen
        if (isDead == false)
            targetHealth += Time.deltaTime * healthRegen;

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

        sliderHealth.maxValue = maxHealth;
        sliderHealthSpeed.maxValue = maxHealth;

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    targetHealth -= 20;
        //}

        if (targetHealth <= -99f)
        {
            targetHealth = -99;
        }
        else if (targetHealth > maxHealth)
        {
            targetHealth = maxHealth;
        }

       
    }

    public void Heal(int healNum)
    {
        targetHealth = Mathf.Clamp(targetHealth + healNum, 0f, maxHealth);
        soundManagerScript.instance.PlayOneShot("reward");
    }
    public void takeDamage(int damage, float speed = 11)
    {
        if (character.isCheatMode)
        {
            return;
        }

        if (enabled == false)
        {
            return;
        }

        if (healthAfterHeal > 0)
        {
            healthAfterHeal -= damage;
        }
        targetHealth -= (damage/ armorStrength);

        if (dof.focalLength.value < 68f)
        {
            dof.focalLength.value = damage * 20f;

            if (dof.focalLength.value > 68f)
            {
                dof.focalLength.value = 68f;
            }
        }

        float random = Random.Range(0f, 1f);

        if (random > 0.5f)
        {
            soundManagerScript.instance.PlayOneShot("hurt.0");
        }
        else
        {
            soundManagerScript.instance.PlayOneShot("hurt.1");
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

            if (character.tutorialMode == false) FPSMainScript.instance.PlayerDie();
            deathCamera.gameObject.SetActive(true);
            cameraScript.instance.gameObject.SetActive(false);
            FPSMainScript.instance.Death_NoSaturation();
            MainGameHUDScript.Instance.gameObject.SetActive(false);

            soundManagerScript.instance.Play("Die");

            if (MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.StopMusic();
            }
        }

        die = true;
    }
}
