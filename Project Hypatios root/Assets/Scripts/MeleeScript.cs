using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScript : MonoBehaviour
{
    
    Camera cam;
    public float meleeDamage;
    public float meleeRange;
    public AudioSource audio_HitAttacked;
    public AudioSource audio_Swing;
    float meleeFrame = 15f;
    float meleeTime;
    float curMeleeTime;
    int weaponBeforeMelee;
    Animator meleeAnim;
    bool hasMeleed;
    public bool IsOnMeleeAttack = false;

    WeaponManager WeaponManager;

    // Start is called before the first frame update
    void Start()
    {
        WeaponManager = Hypatios.Player.Weapon;
        cam = Camera.main;
        meleeAnim = GetComponent<Animator>();
        meleeTime = meleeFrame / 30f;
        curMeleeTime = meleeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && !IsOnMeleeAttack)
        {
            IsOnMeleeAttack = true;
            curMeleeTime = meleeTime;
            weaponBeforeMelee = WeaponManager.selectedWeapon;
            WeaponManager.selectedWeapon = -1;
        }

        if (IsOnMeleeAttack)
        {
            Melee();
        }
    }


    public void Melee()
    {
        WeaponManager.unequipWeapon();
        curMeleeTime -= Time.deltaTime;

        //Debug.Log("Meleeing");
        if (!hasMeleed)
        {
            int rand = Random.Range(0, 2);
            if (rand == 1)
            {
                meleeAnim.SetTrigger("melee1");
            }
            else
            {
                meleeAnim.SetTrigger("melee2");
            }
            hasMeleed = true;
            DealMelee();
        }
        if (curMeleeTime <= 0f)
        {
            WeaponManager.selectedWeapon = weaponBeforeMelee;
            WeaponManager.switchWeapon();
            
            IsOnMeleeAttack = false;
            hasMeleed = false;
        }
    }

    void DealMelee()
    {
        RaycastHit hit;
        Vector3 raycastDir = new Vector3(cam.transform.forward.x, cam.transform.forward.y, cam.transform.forward.z);
        audio_Swing.Play();

        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, meleeRange, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(hit.transform.name);
            var damageReceiver = hit.transform.gameObject.GetComponentThenChild<damageReceiver>();

            if (damageReceiver != null)
            {
                var token = new DamageToken();
                token.damage = Random.Range(meleeDamage - 3, meleeDamage + 3);
                if (Hypatios.Player.BonusDamageMelee.Value != 0) token.damage *= Hypatios.Player.BonusDamageMelee.Value;
                damageReceiver.Attacked(token);
                audio_HitAttacked.Play();
                MainGameHUDScript.Instance.audio_CrosshairClick.Play();
            }
        }
    }
}
