using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class gunScript : MonoBehaviour
{
    public Animator anim;
    public string weaponName;

    Camera cam;

    public bool canScope;
    public bool isAutomatic;
    public LayerMask layerMask;
    [Header ("Weapon Status")]
    public float damage;
    public float variableAdditionalDamage = 4f;
    [SerializeField]
    public int totalAmmo;
    [SerializeField]
    public int magazineSize;
    public int curAmmo;
    [Range (0f, .2f)]
    public float spread;
    public float recoilX;
    public float recoilY;
    public float recoilZ;
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

    public weaponManager weaponSystem;
    Recoil gunRecoil;

    public Image crosshairHit;

    // Start is called before the first frame update
    void Start()
    {
        reloadTime = reloadFrame / 60;
        curReloadTime = reloadTime;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //curAmmo = magazineSize;
        anim = GetComponent<Animator>();
        weaponSystem = GameObject.FindGameObjectWithTag("GunHolder").GetComponent<weaponManager>();
        gunRecoil = weaponSystem.gunRecoil;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        anim = weaponSystem.anim;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Holster"))
        {
            HandleReloadInput();
            ShootingInput();
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

    void ShootingInput()
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
                Shoot();
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
                Shoot();
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
    void Shoot()
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
