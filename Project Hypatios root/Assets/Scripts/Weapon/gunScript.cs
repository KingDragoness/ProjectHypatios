using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class GunScript : BaseWeaponScript
{
    [Title("Gun Section")]

    Camera cam;

    public bool canScope;
    public bool isAutomatic;

    public float bulletPerSecond;
    public float repulsionForce = 1;

    public Transform bulletShooter;
    float nextAttackTime = 0f;
    public ParticleSystem muzzle1;
    Ray ray;
    public GameObject bulletImpact;
    public GameObject bulletSparks;
    public GameObject bulletTracer;
    public bool isFiring;
    public bool isReloading = false;
    public float reloadFrame;
    public float reloadTime;
    public float curReloadTime;
    float scopingReload;
    public bool isScoping = false;
    public bool isBurst = false;
    public float burstAmount;

    [Space]

    public AudioSource audioFire;
    public AudioSource audioReload;

    [Space]

    Recoil gunRecoil;

    [Space]
    [Title("Melee Section (Katana)")]
    public bool isMelee = false;
    public bool isMeleeing = false;
    public bool hasHit = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isMelee)
        {
            reloadTime = reloadFrame / 60;
            curReloadTime = reloadTime;
            gunRecoil = weaponSystem.gunRecoil;
        }
        
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //curAmmo = magazineSize;
        anim = GetComponent<Animator>();
        weaponSystem = GameObject.FindGameObjectWithTag("GunHolder").GetComponent<WeaponManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        if (!isMelee)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Holster"))
            {
                HandleReloadInput();
                FireInput();
                Scoping();
            }

            if (isReloading)
            {
                curReloadTime -= Time.deltaTime;
                if (curReloadTime <= 0)
                {
                    int ammoToFill = magazineSize - curAmmo;

                    if ((totalAmmo - ammoToFill) <= 0)
                    {
                        ammoToFill = totalAmmo;
                    }

                    totalAmmo -= ammoToFill;
                    curAmmo += ammoToFill;


                    isReloading = false;
                    curReloadTime = reloadTime;
                }
            }
        }
        else
        {
            Melee();
        }
        
    }

    void Melee()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time >= nextAttackTime)
            {
                isMeleeing = false;
                hasHit = false;
                KatanaSlash();
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
            }
        }
    }

    void KatanaSlash()
    {
        isMeleeing = true;
        anim.SetTrigger("melee");
        Debug.Log("Is Meleeing");
    }

    void HandleReloadInput()
    {
        if (totalAmmo <= 0)
        {
            //soundManagerScript.instance.
            return;
        }

        if (Input.GetButton("Fire1") && curAmmo == 0 && !isReloading && isAutomatic ||
            Input.GetButtonDown("Fire1") && curAmmo == 0 && !isReloading && !isAutomatic ||
            Input.GetKey(KeyCode.R) && curAmmo < magazineSize && !isReloading)
        {

            anim.SetTrigger("reload");
            isReloading = true;


            if (audioReload != null)
            {
                audioReload.Play();
            }
        }
    }

    public override void FireInput()
    {
        if (Input.GetButton("Fire1") && curAmmo > 0 && isAutomatic && !isReloading)
        {
            if (!isFiring)
            {
                isFiring = true;
                anim.SetBool("isFiring", isFiring);
            }

            if (Time.time >= nextAttackTime)
            {
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
                curAmmo--;
            }
        }
        else
        {
            if (isFiring)
            {
                isFiring = false;
                anim.SetBool("isFiring", isFiring);
            }

        }
        if (Input.GetButtonDown("Fire1") && curAmmo > 0 && !isAutomatic && !isReloading)
        {

            if (Time.time >= nextAttackTime)
            {
                anim.SetTrigger("shooting");
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond;
                curAmmo--;
            }
        }
    }

    IEnumerator SetCrosshairHitActive()
    {
        crosshairHit.gameObject.SetActive(true);
        MainGameHUDScript.Instance.audio_CrosshairClick.Play();
        yield return new WaitForSeconds(.2f);
        crosshairHit.gameObject.SetActive(false);
    }

    void Scoping()
    {
        if (Input.GetMouseButton(1) && canScope)
        {
            isScoping = true;

        }
        else
        {
            isScoping = false;
        }

        anim.SetBool("isScoping", isScoping);
    }

    public override void FireWeapon()
    {
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        if (!isBurst)
        {
            FPSMainScript.instance.RuntimeTutorialHelp("SHOOTING", "You can shoot using LMB. Scope using RMB when you obtained a rifle.", "FirstShoot");


            muzzle1.Emit(1);
            var points = new Vector3[2];
            points[0] = bulletShooter.transform.position;
            GameObject trace = Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
            RaycastHit hit;
            float spreadX = Random.Range(-spread, spread);
            float spreadY = Random.Range(-spread, spread);
            Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);


            if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
            {
                var damageReceiver = hit.transform.gameObject.GetComponentInChildren<damageReceiver>();
                float variableDamage = Random.Range(0, variableAdditionalDamage);

                if (damageReceiver != null)
                {
                    damageReceiver.Attacked(damage + variableDamage, repulsionForce);
                    StartCoroutine(SetCrosshairHitActive());
                }

                points[1] = hit.point;
                trace.GetComponent<LineRenderer>().SetPositions(points);
                Destroy(trace, .03f);
                if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                {
                    GameObject bulletHole = Instantiate(bulletImpact, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                    bulletHole.transform.SetParent(hit.collider.gameObject.transform);

                    Destroy(bulletHole, 4f);
                }

                GameObject bulletSpark_ = Instantiate(bulletSparks, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletSpark_, 4f);

            }
            else
            {
                points[1] = cam.ViewportToWorldPoint(new Vector3(.5f + spreadX, .5f + spreadY, 100f));
                trace.GetComponent<LineRenderer>().SetPositions(points);
                Destroy(trace, .03f);
            }

        }
        else
        {
            StopCoroutine(PlaySoundBurst());
            StartCoroutine(PlaySoundBurst());

            muzzle1.Emit(1);
            var points = new Vector3[2];
            points[0] = bulletShooter.transform.position;
            RaycastHit hit;
            for (int i=0; i<burstAmount; i++)
            {
                GameObject trace = Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
                float spreadX = Random.Range(-spread, spread);
                float spreadY = Random.Range(-spread, spread);
                Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);

                if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
                {
                    var damageReceiver = hit.transform.gameObject.GetComponentInChildren<damageReceiver>();
                    float variableDamage = Random.Range(0, variableAdditionalDamage);

                    if (damageReceiver != null)
                    {
                        damageReceiver.Attacked(damage + variableDamage, repulsionForce);
                        StartCoroutine(SetCrosshairHitActive());
                    }

                    points[1] = hit.point;
                    trace.GetComponent<LineRenderer>().SetPositions(points);
                    Destroy(trace, .03f);

                    if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                    {
                        GameObject bulletHole = Instantiate(bulletImpact, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                        bulletHole.transform.SetParent(hit.collider.gameObject.transform);

                        Destroy(bulletHole, 4f);
                    }

                    GameObject bulletSpark_ = Instantiate(bulletSparks, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bulletSpark_, 4f);
                }
                else
                {
                    points[1] = cam.ViewportToWorldPoint(new Vector3(.5f + spreadX, .5f + spreadY, 100f));
                    points[1] = points[1] + new Vector3(spreadX, spreadY, 0f);
                    trace.GetComponent<LineRenderer>().SetPositions(points);
                    Destroy(trace, .03f);
                }

            }
            
        }
        
        IEnumerator PlaySoundBurst()
        {
            //Multiple gunshot
            if (audioFire == null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
            audioFire.PlayOneShot(audioFire.clip);
            yield return new WaitForSeconds(0.05f);
            audioFire.PlayOneShot(audioFire.clip);

        }
    }   
}
