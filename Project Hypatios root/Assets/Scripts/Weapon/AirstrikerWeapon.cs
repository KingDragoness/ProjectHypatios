using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AirstrikerWeapon : GunScript
{

    public Transform strikeGunner;
    public ModularTurretGun modularGunner; //to be removed/replaced
    public GameObject pingStrike;
    public GameObject directPing;
    public float errorAccuracy = 2f;
    public float InitializationTime = 5f;
    public float limitDistanceTarget = 3f;

    [FoldoutGroup("Audios")] public AudioSource audio_Bootup;
    [FoldoutGroup("Audios")] public AudioSource audio_Startup;
    [FoldoutGroup("Monitor")] public GameObject screen_Online;
    [FoldoutGroup("Monitor")] public TextMesh screen_consoleText;
    [FoldoutGroup("Monitor")] public GameObject screen_CannotTarget;
    [FoldoutGroup("Monitor")] public GameObject screen_MonitorToTarget;

    private int limitLine = 8;
    private List<string> allLinesConsole = new List<string>();
    private float timerInitialize = 5f;
    private bool canFireTarget = false;
    private bool hasInitialized = false;
    [SerializeField] private bool isLocking = false;

    private void OnEnable()
    {
        if (hasInitialized)
        {
            pingStrike.gameObject.SetActive(true);
            directPing.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        pingStrike.gameObject.SetActive(false);
        directPing.gameObject.SetActive(false);

    }

    public override void Start()
    {
        base.Start();
        audio_Bootup.Play();
        timerInitialize = InitializationTime;
        strikeGunner.SetParent(null);
        pingStrike.transform.SetParent(null);
        directPing.transform.SetParent(null);
        screen_Online.gameObject.SetActive(false);
        pingStrike.gameObject.SetActive(false);
        directPing.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (strikeGunner != null)
            Destroy(strikeGunner.gameObject);

        if (pingStrike != null)
            Destroy(pingStrike.gameObject);

        if (directPing != null)
            Destroy(directPing.gameObject);
    }


    #region Update Systems

    public override void Update()
    {
        base.Update();
        UpdateMonitor();

        if (timerInitialize > 0)
        {
            timerInitialize -= Time.deltaTime;
            return;
        }

        if (hasInitialized == false)
        {
            InsertLine("READY", 999);
            audio_Bootup.Stop();
            audio_Startup.Play();
            pingStrike.gameObject.SetActive(true);
            directPing.gameObject.SetActive(true);
            OnlineMonitorActive();
            hasInitialized = true;
        }

        if (isReloading)
        {
            float percentage = (ReloadTime - curReloadTime) / ReloadTime; percentage *= 100f;
            if (percentage >= 100) percentage = 100;
            InsertLine($"Rearming...{Mathf.Floor(percentage)}%", 0);
        }
    }

    private void OnlineMonitorActive()
    {
        screen_Online.gameObject.SetActive(true);
    }

    private void UpdateMonitor()
    {
        if (hasInitialized == false)
        {
            float percentage = ((InitializationTime - timerInitialize) / InitializationTime) * 100f;
            if (percentage >= 100) percentage = 100;
            InsertLine($"Initializing...{Mathf.Floor(percentage)}%", 0);
        }

        string s = "";
        List<string> reversedList = new List<string>();
        reversedList.AddRange(allLinesConsole);
        reversedList.Reverse();

        foreach(var line in reversedList)
        {
            if (string.IsNullOrEmpty(line) == false)
                s += $"{line} <\n";
        }

        screen_consoleText.text = s;

        if (canFireTarget)
            screen_CannotTarget.gameObject.SetActive(false);
        else
            screen_CannotTarget.gameObject.SetActive(true);

        if (isLocking)
        {
            screen_MonitorToTarget.gameObject.SetActive(false);
        }
        else
        {
            screen_MonitorToTarget.gameObject.SetActive(true);
        }

    }

    private void InsertLine(string line, int index = 0)
    {
        allLinesConsole.RemoveAll(x => x == null);

        if (allLinesConsole.Count == 0)
        {
            allLinesConsole.Insert(0, line);
        }
        else if (allLinesConsole.Count < index)
        {
            allLinesConsole.Insert(0, line);
            allLinesConsole.Insert(0, string.Empty);
        }
        else
        {
            allLinesConsole[index] = line;
        }

        if (allLinesConsole.Count > limitLine)
        {
            for (int v = allLinesConsole.Count - 1; v > limitLine; v--)
            {
                string line12 = allLinesConsole[v];

                if (string.IsNullOrEmpty(line12) == true)
                    continue;

                if (allLinesConsole.Count > v)
                    allLinesConsole.RemoveAt(v);
            }
        }
    }

    #endregion


    #region Weapon Firing

    public RaycastHit GetDroneHit(float range = 1000f)
    {
        RaycastHit hit;
        Vector3 raycastDir = Vector3.down;
        Vector3 posShooter = PosShooter();

        if (Physics.Raycast(posShooter, raycastDir, out hit, range, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
        }
        else
        {
            hit.point = posShooter + (raycastDir * range);
        }

        return hit;

    }

    public RaycastHit GetCounterDroneHit(Vector3 droneHit, float range = 200f)
    {
        RaycastHit hit;
        Vector3 raycastDir = Vector3.up;
        Vector3 posShooter = droneHit;

        if (Physics.Raycast(posShooter, raycastDir, out hit, range, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
        }
        else
        {
            hit.point = posShooter + (raycastDir * range);
        }

        return hit;

    }

    public override void FireInput()
    {
        if (hasInitialized == false) return;
        base.FireInput();

        if (Hypatios.Input.Fire2.triggered)
        {
            ToggleLock();
        }
    }

    private void ToggleLock()
    {
        if (isLocking)
        {
            isLocking = false;
            InsertLine("UNLOCKED", 999);
        }
        else
        {
            isLocking = true;
            InsertLine("LOCK", 999);
        }
    }

    public Vector3 PosShooter()
    {
        Vector3 posShooter = directPing.transform.position;
        posShooter.y += 200f;

        return posShooter;
    }

    private bool dontShoot = false;

    private void FixedUpdate()
    {
        if (hasInitialized == false)
            return;

        RaycastHit cameraHit = GetHit(300f);
        if (isLocking == false) directPing.transform.position = cameraHit.point;

        RaycastHit droneHit = GetDroneHit();
        RaycastHit counterDroneHit = GetCounterDroneHit(droneHit.point + droneHit.normal * 0.2f, 200f);
        RaycastHit finalHit = droneHit;
        bool hittedCeiling = false;

        if (counterDroneHit.collider != null)
        {
            hittedCeiling = true;
            finalHit = counterDroneHit;
        }

        Entity anyEntityInCircle = Hypatios.Enemy.FindEnemyEntity(Alliance.Player, finalHit.point, 0f, errorAccuracy);
        float distHitbyHit = Vector3.Distance(directPing.transform.position, finalHit.point);
        bool resizeParticle = false;
        bool isEnemyOnCircle = false;
        dontShoot = false;

        if (distHitbyHit > limitDistanceTarget)
            canFireTarget = false;
        else
            canFireTarget = true;

        if (anyEntityInCircle != null)
            isEnemyOnCircle = true;

   
        if (isFiring)
        {
            Vector3 posShooter = PosShooter();
            posShooter.x += Random.Range(-5f, 5f);
            posShooter.z += Random.Range(-5f, 5f);
            strikeGunner.transform.position = posShooter;

            if (cameraHit.collider != null && isLocking == false)
            {
                EnemyScript enemy = cameraHit.collider.GetComponentInParent<EnemyScript>();

                Vector3 targetLook = cameraHit.point;
                targetLook.x += Random.Range(-errorAccuracy, errorAccuracy);
                targetLook.z += Random.Range(-errorAccuracy, errorAccuracy);
                strikeGunner.transform.LookAt(targetLook);

                if (enemy != null)
                {
                    CorrectRotationToEntity(enemy);
                    resizeParticle = true;
                }
            }
            else if (isLocking == true)
            {
                Vector3 targetLook = directPing.transform.position;
                targetLook.x += Random.Range(-errorAccuracy, errorAccuracy);
                targetLook.z += Random.Range(-errorAccuracy, errorAccuracy);
                strikeGunner.transform.LookAt(targetLook);
            }
            else
            {
                dontShoot = true;
            }

            if (isEnemyOnCircle)
            {
                Vector3 targetLook = anyEntityInCircle.OffsetedBoundWorldPosition;
                strikeGunner.transform.LookAt(targetLook);
            }
        }
        else
        {
            dontShoot = true;
        }

        if (dontShoot)
            modularGunner.enabled = false;
        else
            modularGunner.enabled = true;

        if (resizeParticle)
            pingStrike.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
        else
            pingStrike.transform.localScale = new Vector3(2f, 2f, 2f);

        if (isLocking == false)
        {
            if (canFireTarget)
                pingStrike.transform.position = cameraHit.point;
            else
                pingStrike.transform.position = finalHit.point;

        }


        directPing.transform.eulerAngles = Vector3.zero;
        pingStrike.transform.eulerAngles = Vector3.zero;
    }

    private void FireGunner()
    {

    }

    public override void FireWeapon()
    {

        //enable strikeTarget;

    }

    #endregion

    private void CorrectRotationToEntity(Entity entity)
    {
        strikeGunner.transform.LookAt(entity.OffsetedBoundWorldPosition);
    }

    //Called from unityevent
    public void InitiateReload()
    {
    }

    public override void OnReloadCompleted()
    {
        InsertLine("READY", 999);
        audio_Startup.Play();
        base.OnReloadCompleted();
    }


}
