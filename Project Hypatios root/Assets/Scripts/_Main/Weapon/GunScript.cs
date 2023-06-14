using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class GunScript : BaseWeaponScript
{



    public float repulsionForce = 1;
    public bool isFiring;
    public bool isReloading = false;
    public bool canScope;
    public bool isAutomatic;
    public float reloadFrame;
    public float curReloadTime;
    public bool isScoping = false;
    public bool isBurst = false;
    public float burstAmount;

    protected internal float reloadTime;
    protected internal float scopingReload;
    protected internal float nextAttackTime = 0f;
    protected internal RaycastHit currentHit;


    [FoldoutGroup("References")] public Transform bulletShooter;
    [FoldoutGroup("References")] public ParticleSystem muzzle1;
    [FoldoutGroup("References")] public GameObject bulletImpact;
    [FoldoutGroup("References")] public GameObject bulletSparks;
    [FoldoutGroup("References")] public GameObject bulletTracer;

    [FoldoutGroup("Extensions")] public UnityEvent OnFire;
    [FoldoutGroup("Extensions")] public UnityEvent OnHolding;
    [FoldoutGroup("Extensions")] public UnityEvent OnReloadStart;
    [FoldoutGroup("Extensions")] public Action<string> OnFireAction;

    [FoldoutGroup("Audios")] public AudioSource audioFire;
    [FoldoutGroup("Audios")] public AudioSource audioReload;

    [FoldoutGroup("Katana")] public bool isMelee = false;
    [FoldoutGroup("Katana")] public bool isMeleeing = false;
    [FoldoutGroup("Katana")] public bool hasHit = false;
    public float ReloadTime { get => reloadTime; }

    protected internal Camera cam;
    protected internal Ray ray;
    protected internal Recoil gunRecoil;
    private float justUnpaused = 0.1f; //absolutely retarded fix

    public WeaponItem GetWeaponItem()
    {
        return Hypatios.Assets.GetWeapon(weaponName);
    }

    public HypatiosSave.WeaponDataSave GetWeaponItemSave()
    {
        return Hypatios.Game.GetWeaponSave(weaponName);
    }

    protected internal bool IsRecentlyPaused()
    {
        if (justUnpaused <= 0f)
            return true;

        return false;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        var item = Hypatios.Assets.GetItemByWeapon(weaponName);
        Hypatios.Game.RuntimeTutorialHelp(item.GetDisplayText(), item.Description, $"Weapon.{weaponName}");
        weaponSystem = GameObject.FindGameObjectWithTag("GunHolder").GetComponent<WeaponManager>();

        if (!isMelee)
        {
            reloadTime = reloadFrame / 60;
            curReloadTime = ReloadTime;
            gunRecoil = weaponSystem.Recoil;
        }
        
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //curAmmo = magazineSize;
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        justUnpaused -= Time.deltaTime;

        if (Time.timeScale <= 0)
        {
            justUnpaused = 0.1f;
            return;
        }

        if (Hypatios.Player.disableInput)
            return;


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
                    OnReloadCompleted();

                    isReloading = false;
                    curReloadTime = ReloadTime;
                }
            }
        }
        else
        {
            Melee();
        }
        
    }

    public virtual void OnReloadCompleted()
    {

    }

    void Melee()
    {
        if (Hypatios.Input.Fire1.triggered)
        {
            if (Time.time >= nextAttackTime)
            {
                isMeleeing = false;
                hasHit = false;
                KatanaSlash();
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.03f;
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

        if (Hypatios.Input.Fire1.IsPressed() && curAmmo == 0 && !isReloading && isAutomatic ||
            Hypatios.Input.Fire1.triggered && curAmmo == 0 && !isReloading && !isAutomatic ||
            Hypatios.Input.Reload.triggered && curAmmo < magazineSize && !isReloading)
        {
            OnReloadStart?.Invoke();
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
        if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && isAutomatic && !isReloading)
        {
            if (!isFiring)
            {
                isFiring = true;
                anim.SetBool("isFiring", isFiring);
            }

            if (Time.time >= nextAttackTime)
            {
                FireWeapon();
                nextAttackTime = Time.time + 1f / bulletPerSecond + 0.03f;
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
        if (Hypatios.Input.Fire1.triggered && curAmmo > 0 && !isAutomatic && !isReloading)
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

    public void HandleCrosshairActive(damageReceiver damageReceiver)
    {
        if (damageReceiver.destructibleScript != null)
        {
            if (damageReceiver.destructibleScript.destroyType == damageType)
                StartCoroutine(SetCrosshairHitActive());
        }
        else
            StartCoroutine(SetCrosshairHitActive());
    }

    protected internal IEnumerator SetCrosshairHitActive()
    {
        crosshairHit.gameObject.SetActive(true);
        MainGameHUDScript.Instance.audio_CrosshairClick.Play();
        yield return new WaitForSeconds(.2f);
        crosshairHit.gameObject.SetActive(false);
    }

    void Scoping()
    {
        if (Hypatios.Input.Fire2.IsPressed() && canScope)
        {
            OnHolding?.Invoke();
            isScoping = true;

        }
        else
        {
            isScoping = false;
        }

        anim.SetBool("isScoping", isScoping);
    }

    IEnumerator shotgunCoroutine;

    public RaycastHit GetHit(float range = 1000f)
    {
        RaycastHit hit;
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);


        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, range, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
        }
        else
        {
            hit.point = cam.transform.position + (raycastDir * range);
        }

        return hit;

    }

    private int loopScatterAmount = 0;

    public void FireAdditionalScatterBullets(float spreadMult = 1f, int amount = 1)
    {
        if (loopScatterAmount > 100)
        {
            Debug.LogError("Stack overflow");
            return;
        }

        if (loopScatterAmount < amount)
        {
            loopScatterAmount++;
            FireAdditionalScatterBullets(spreadMult, amount);
        }
        else
        {
            loopScatterAmount = 0;
            return;
        }

        var damageToken = new DamageToken();
        damageToken.isBurn = isBurnBullet;
        damageToken.isPoison = isPoisonBullet;
        damageToken.damageType = DamageToken.DamageType.Ballistic;

        RaycastHit hit;
        float spreadX = Random.Range(-spread * spreadMult, spread * spreadMult);
        float spreadY = Random.Range(-spread * spreadMult, spread * spreadMult);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);


        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
            float variableDamage = Random.Range(0, variableAdditionalDamage);

            if (damageReceiver != null)
            {
                damageToken.damage = (damage * Hypatios.Player.BonusDamageGun.Value / 5f) + variableDamage; damageToken.repulsionForce = repulsionForce;
                UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
                HandleCrosshairActive(damageReceiver);
            }

            if (hit.transform.gameObject.layer != 13 &&
                    hit.transform.gameObject.layer != 12)
            {
                GameObject bulletHole = Hypatios.ObjectPool.SummonObject(bulletImpact, 10, IncludeActive: true);
                if (bulletHole != null)
                {
                    bulletHole.transform.position = hit.point + hit.normal * .0001f;
                    bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                    bulletHole.transform.SetParent(hit.collider.gameObject.transform);
                }
            }

            GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(bulletSparks, 10, IncludeActive: true);
            if (bulletSpark_ != null)
            {
                bulletSpark_.transform.position = hit.point;
                bulletSpark_.transform.rotation = Quaternion.LookRotation(hit.normal);
                bulletSpark_.DisableObjectTimer(2f);
            }

            currentHit = hit;
        }
    }

    public override void FireWeapon()
    {
        loopScatterAmount = 0;
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        var damageToken = new DamageToken();
        damageToken.isBurn = isBurnBullet;
        damageToken.isPoison = isPoisonBullet;
        damageToken.origin = DamageToken.DamageOrigin.Player;
        damageToken.damageType = damageType;
        OnFire?.Invoke();
        OnFireAction?.Invoke($"{weaponName}");

        if (!isBurst)
        {

            muzzle1.Emit(1);
            var points = new Vector3[2];
            points[0] = bulletShooter.transform.position;
            GameObject trace = Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
            RaycastHit hit;
            float spreadX = Random.Range(-spread, spread);
            float spreadY = Random.Range(-spread, spread);
            Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);
           

            if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
            {
                var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
                float variableDamage = Random.Range(0, variableAdditionalDamage);

                if (damageReceiver != null)
                {
                    damageToken.damage = damage * Hypatios.Player.BonusDamageGun.Value + variableDamage; damageToken.repulsionForce = repulsionForce;
                    UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
                    HandleCrosshairActive(damageReceiver);
                }

                points[1] = hit.point;
                trace.GetComponent<LineRenderer>().SetPositions(points);
                Destroy(trace, .03f);
                if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                {
                    GameObject bulletHole = Hypatios.ObjectPool.SummonObject(bulletImpact, 10, IncludeActive: true);
                    if (bulletHole != null)
                    {
                        bulletHole.transform.position = hit.point + hit.normal * .0001f;
                        bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                        //bulletHole.DisableObjectTimer(2f);
                        bulletHole.transform.SetParent(hit.collider.gameObject.transform);
                    }

                }

                GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(bulletSparks, 10, IncludeActive: true);
                if (bulletSpark_ != null)
                {
                    bulletSpark_.transform.position = hit.point;
                    bulletSpark_.transform.rotation = Quaternion.LookRotation(hit.normal);
                    bulletSpark_.DisableObjectTimer(2f);
                }
                //Instantiate(bulletSparks, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(bulletSpark_, 4f);

                currentHit = hit;
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
            if (shotgunCoroutine != null) StopCoroutine(shotgunCoroutine);
            shotgunCoroutine = PlaySoundBurst();
            StartCoroutine(shotgunCoroutine);

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

                if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 1000f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
                {
                    var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
                    float variableDamage = Random.Range(0, variableAdditionalDamage);

                    if (damageReceiver != null)
                    {
                        damageToken.damage = (damage * Hypatios.Player.BonusDamageGun.Value / 2f) + variableDamage; damageToken.repulsionForce = repulsionForce;
                        UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
                        HandleCrosshairActive(damageReceiver);
                    }

                    points[1] = hit.point;
                    trace.GetComponent<LineRenderer>().SetPositions(points);
                    Destroy(trace, .03f);

                    if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                    {
                        GameObject bulletHole = Hypatios.ObjectPool.SummonObject(bulletImpact, 10, IncludeActive: true);
                        if (bulletHole != null)
                        {
                            bulletHole.transform.position = hit.point + hit.normal * .0001f;
                            bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                            bulletHole.transform.SetParent(hit.collider.gameObject.transform);
                        }

                    }

                    GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(bulletSparks, 10, IncludeActive: true);
                    if (bulletSpark_ != null)
                    {
                        bulletSpark_.transform.position = hit.point;
                        bulletSpark_.transform.rotation = Quaternion.LookRotation(hit.normal);
                        bulletSpark_.DisableObjectTimer(2f);
                    }

                    currentHit = hit;
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

            //yield return new WaitForSeconds(0.05f);
            audioFire.Play();
            //yield return new WaitForSeconds(0.05f);
            //audioFire.PlayOneShot(audioFire.clip);

        }
    }   
}
