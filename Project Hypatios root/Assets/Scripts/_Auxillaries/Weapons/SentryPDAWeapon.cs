using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;


public class SentryPDAWeapon : GunScript
{
    public enum Mode
    {
        Build,
        Repair,
        Control,
        Destroy
    }

    [FoldoutGroup("Sentry PDA")] public TextMesh DigitOutputLabel;
    [FoldoutGroup("Sentry PDA")] public TextMesh Label_WrenchAmmo;
    [FoldoutGroup("Sentry PDA")] public TextMesh Label_SentryAmmo;
    [FoldoutGroup("Sentry PDA")] public TextMesh Label_HP;
    [FoldoutGroup("Sentry PDA")] public TextMesh Label_Prompt;
    [FoldoutGroup("Sentry PDA")] public GameObject weaponIcon;
    [FoldoutGroup("Sentry PDA")] public Mode currentMode;
    [FoldoutGroup("Sentry PDA")] public GameObject TargetSentryBuild;
    [FoldoutGroup("Sentry PDA")] public GameObject PingAim;
    [FoldoutGroup("Sentry PDA")] public Transform outLine;
    [FoldoutGroup("Sentry PDA")] public LineRenderer targetLineRendr;
    [FoldoutGroup("Build Sentry")] public Fortification_GhostSentry SentryPrefab;
    [FoldoutGroup("Build Sentry")] public GameObject validSentryHighlight;
    [FoldoutGroup("Build Sentry")] public GameObject invalidSentryHighlight;
    [FoldoutGroup("Build Sentry")] public Transform[] buildCheckPoints;
    [FoldoutGroup("Build Sentry")] public float groundCheckDist = 0.16f;
    [FoldoutGroup("Audios")] public AudioSource audio_ClickCycle;
    [FoldoutGroup("Audios")] public AudioSource audio_Processing;
    [FoldoutGroup("Audios")] public AudioSource audio_HackingFailed;

    public Fortification_GhostSentry currentSentryGun;
    public ItemInventory itemRareMetal;
    public ItemInventory itemMicrochip;

    private float _intervalRepair = 2;
    private float _timerToDestroy = 3f;
    private float _timerToBuild = 2f;
    private bool isDestroyingHold = false;
    private bool isBuildHold = false;
    private bool _isValidBuild = false;

    private float perHealAmount = 1f;
    private int perAmmoAmount = 1;
    private int cost_metal = 1;
    private int cost_raremetal = 1;
    private Transform[] _targetedGrounds;
    [ShowInInspector] [ReadOnly] private Transform _mainGround;

    public bool IsSentryActive
    {
        get
        {
            return currentSentryGun != null;
        }
    }

    public override void Start()
    {
        _targetedGrounds = new Transform[buildCheckPoints.Length];
        perAmmoAmount = Mathf.RoundToInt(GetFinalValue("AmmoAmount"));
        perHealAmount = GetFinalValue("HealAmount");
        cost_metal = Mathf.RoundToInt(GetFinalValue("CommonMetalCost"));
        cost_raremetal = Mathf.RoundToInt(GetFinalValue("RareMetalCost"));
        TargetSentryBuild.transform.SetParent(null);
        base.Start();
    }

    private void OnEnable()
    {
        if (Fortification_GhostSentry.Instance != null)
            currentSentryGun = Fortification_GhostSentry.Instance;

        TargetSentryBuild.gameObject.SetActive(true);

    }

    private void OnDisable()
    {
        TargetSentryBuild.gameObject.SetActive(false);
        PingAim.gameObject.SetActive(false);

        if (currentSentryGun != null)
            currentSentryGun.manualControl = false;
    }

    private void OnDestroy()
    {
        if (TargetSentryBuild != null)
            Destroy(TargetSentryBuild.gameObject);
    }

    #region Updates
    public override void Update()
    {
        base.Update();

        if (Time.timeScale <= 0) return;

        if (Hypatios.Input.Fire2.WasReleasedThisFrame() && !isReloading && IsRecentlyPaused())
            CycleMode();

        DisplaySentryMonitor();
        EnforceChangeState();
        RunBehaviour();
    }

    private void CycleMode()
    {
        audio_ClickCycle.Play();
        if (IsSentryActive)
        {
            int i = (int)currentMode;
            i++;
            if (i > 3)
                i = 1;
            currentMode = (Mode)i;
        }
        else
        {
            currentMode = Mode.Build;
        }
    }

    private void RunBehaviour()
    {
        if (currentMode == Mode.Build)
        {
            if (!TargetSentryBuild.gameObject.activeSelf) TargetSentryBuild.gameObject.SetActive(true);
            Run_BuildMode();
        }
        else if (currentMode == Mode.Control)
        {
            if (!PingAim.gameObject.activeSelf) PingAim.gameObject.SetActive(true);
            Run_ControlMode();
        }
        else if (currentMode == Mode.Destroy)
        {
            Run_DestroyMode();
        }
        else if (currentMode == Mode.Repair)
        {
            Run_RepairMode();
        }

        if (currentMode != Mode.Build)
        {
            if (TargetSentryBuild.gameObject.activeSelf) TargetSentryBuild.gameObject.SetActive(false);
        }

        if (currentMode != Mode.Control)
        {
            if (currentSentryGun != null) currentSentryGun.manualControl = false;

            if (PingAim.gameObject.activeSelf) PingAim.gameObject.SetActive(false);

        }
    }

    private void Run_BuildMode()
    {
        var hitBuild = GetHit();
        TargetSentryBuild.transform.position = hitBuild.point;
        Vector3 rot = transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        TargetSentryBuild.transform.eulerAngles = rot;

        _isValidBuild = IsValidToBuild();
        _mainGround = GetDominantGround();

        if (_mainGround == null) _isValidBuild = false;
        if (IsBuildCost() == false) _isValidBuild = false;
             
        if (_isValidBuild)
        {
            validSentryHighlight.gameObject.SetActive(true);
            invalidSentryHighlight.gameObject.SetActive(false);
        }
        else
        {
            validSentryHighlight.gameObject.SetActive(false);
            invalidSentryHighlight.gameObject.SetActive(true);
        }
    }

    private void Run_ControlMode()
    {
        var hitAim = GetHit();

        currentSentryGun.manualControl = true;
        currentSentryGun.OverrideTarget(hitAim.point);
        PingAim.transform.position = hitAim.point;
    }

    private void Run_DestroyMode()
    {

    }

    private void Run_RepairMode()
    {

    }

    private void EnforceChangeState()
    {
        if (currentMode != Mode.Build && currentSentryGun == null)
        {
            currentMode = Mode.Build;
        }
        else if (currentMode == Mode.Build && currentSentryGun != null)
        {
            currentMode = Mode.Repair;
        }
    }


    private void DisplaySentryMonitor()
    {
        if (currentSentryGun == null)
        {
            weaponIcon.gameObject.SetActive(false);
            targetLineRendr.gameObject.SetActive(false);
            Label_HP.gameObject.SetActive(false);
            Label_SentryAmmo.gameObject.SetActive(false);
        }
        else
        {
            Vector3[] v3 = new Vector3[2];
            v3[0] = outLine.transform.position;
            v3[1] = currentSentryGun.transform.position;
            targetLineRendr.SetPositions(v3);
            targetLineRendr.gameObject.SetActive(true);
            weaponIcon.gameObject.SetActive(true);

            Label_HP.gameObject.SetActive(true);
            Label_SentryAmmo.gameObject.SetActive(true);
            Label_HP.text = $"{Mathf.RoundToInt(currentSentryGun.Stats.CurrentHitpoint)}/{Mathf.RoundToInt(currentSentryGun.Stats.MaxHitpoint.Value)}";
            Label_SentryAmmo.text = $"x{currentSentryGun.sentryAmmo}";
        }

        {
            if (currentMode == Mode.Build)
            {
                DigitOutputLabel.text = $"BUIL.D000.MODE";
                if (isBuildHold)
                {
                    Label_Prompt.text = $"<HOLD... ({Mathf.RoundToInt(_timerToBuild * 10f) / 10f}/2s)>";
                }
                else
                {
                    if (IsBuildCost() == true)
                        Label_Prompt.text = $"<LMB to Build>";
                    else
                        Label_Prompt.text = $"<Need {cost_metal} microchip & {cost_raremetal} rare metal!>";
                }
            }
            else if (currentMode == Mode.Control)
            {
                DigitOutputLabel.text = $"CTRL.0000.MODE";
                Label_Prompt.text = $"<LMB to Shoot>";
            }
            else if (currentMode == Mode.Destroy)
            {
                DigitOutputLabel.text = $"DEST.ROY0.MODE";
                if (isDestroyingHold)
                {
                    Label_Prompt.text = $"<HOLD... ({Mathf.RoundToInt(_timerToDestroy*10f)/10f}/2s)>";
                }
                else
                {
                    Label_Prompt.text = $"<LMB to Destroy>";
                }
            }
            else if (currentMode == Mode.Repair)
            {
                DigitOutputLabel.text = $"REPA.IR00.MODE";
                Label_Prompt.text = $"<LMB to Repair>";
            }
        }

        float percent = Mathf.Floor(((float)curAmmo / (float)magazineSize) * 100f);
        Label_WrenchAmmo.text = $"{percent}%";


    }

    #endregion

    #region Build Systems

    public bool IsBuildCost()
    {
        int commonMetal = Hypatios.Player.Inventory.Count(itemMicrochip);
        int rareMetal = Hypatios.Player.Inventory.Count(itemRareMetal);

        if (commonMetal >= cost_metal && rareMetal >= cost_raremetal)
        {
            return true;
        }
        return false;
    }

    public bool IsValidToBuild()
    {
        bool valid = true;

        for (int i = 0; i < _targetedGrounds.Length; i++)
        {
            _targetedGrounds[i] = null;
        }

        for(int i = 0; i < buildCheckPoints.Length; i++)
        {
            Transform t = buildCheckPoints[i];
            RaycastHit hit;
            Vector3 dir = Vector3.down;

            //ground check
            if (Physics.Raycast(t.transform.position, dir, out hit, groundCheckDist, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(t.transform.position, dir * hit.distance, Color.blue);
                _targetedGrounds[i] = hit.collider.transform;
            }
            else
            {
                Debug.DrawRay(t.transform.position, dir * groundCheckDist, Color.blue);
                valid = false;
            }

            //point to point check
            int i_1 = i + 1;
            if (i_1 >= buildCheckPoints.Length)
                i_1 = 0;

            Transform t1 = buildCheckPoints[i_1];
            Vector3 dir1 = t1.transform.position - t.transform.position;
            dir1.Normalize();
            float dist1 = Vector3.Distance(t.transform.position, t1.transform.position);

            if (Physics.Raycast(t.transform.position, dir1, out hit, dist1, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(t.transform.position, dir1 * hit.distance, Color.blue);
                valid = false;
            }
        }

        return valid;
    }

    public Transform GetDominantGround()
    {
        var list1 = _targetedGrounds.ToList();
        int highestCount = 0;
        int currentIndex = 0;

        for (int i = 0; i < _targetedGrounds.Length; i++)
        {
            int count1 = list1.Where(x => x == _targetedGrounds[i]).Count();
            if (highestCount < count1)
            {
                highestCount = count1;
                currentIndex = i;
            }
        }

        return _targetedGrounds[currentIndex];
    }

    #endregion

    #region Actions


    public override void FireInput()
    {
        //base.FireInput();
        bool isProcessing = false;
        isDestroyingHold = false;
        isBuildHold = false;

        if (Hypatios.Input.Fire1.IsPressed() && !isReloading && IsRecentlyPaused())
        {
            if (currentMode == Mode.Control)
            {
                Action_Control();
            }
        }

        if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && !isReloading && IsRecentlyPaused())
        {
            if (currentMode == Mode.Repair)
            {
                Action_Repair();
                if (ValidRepair()) isProcessing = true;
            }
        }

        if (Hypatios.Input.Fire1.IsPressed() && !isReloading && IsRecentlyPaused())
        {
            if (currentMode == Mode.Destroy)
            {
                isDestroyingHold = true;
                isProcessing = true;
            }
        }

        if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && !isReloading && IsRecentlyPaused())
        {
            //anim.SetTrigger("Fire");

            if (currentMode == Mode.Build && _isValidBuild)
            {
                isBuildHold = true;
                isProcessing = true;
            }
        }


        if (isDestroyingHold)
        {
            _timerToDestroy -= Time.deltaTime;

            if (_timerToDestroy <= 0)
            {
                Action_Destroy();
            }
        }
        else
        {
            _timerToDestroy = 2f;
        }


        if (isBuildHold)
        {
            _timerToBuild -= Time.deltaTime;

            if (_timerToBuild <= 0)
            {
                Action_Build();
            }
        }
        else
        {
            _timerToBuild = 2f;
        }


        if (isProcessing)
        {
            Audio_Processing(true);
        }
        else
        {
            Audio_Processing(false);
            _intervalRepair = 1f / bulletPerSecond + 0.02f;
        }

        #region Unused
        /*
        if (Hypatios.Input.Fire1.WasReleasedThisFrame() && curAmmo > 0 && !isReloading && IsRecentlyPaused()
        && currentSentryGun != null)
        {
            anim.SetTrigger("Fire");

            if (currentMode == Mode.Build)
                Action_Build();
            else if (currentMode == Mode.Control)
                Action_Control();
            else if (currentMode == Mode.Repair)
                Action_Repair();
            else if (currentMode == Mode.Destroy)
                Action_Destroy();
        }
        */
        #endregion

    }

    public bool ValidRepair()
    {
        bool isHPMax = false;
        bool isAmmoMax = false;

        if (currentSentryGun.sentryAmmo >= currentSentryGun.maxSentryAmmo)
            isAmmoMax = true;

        if (currentSentryGun.Stats.CurrentHitpoint >= currentSentryGun.Stats.MaxHitpoint.Value)
            isHPMax = true;

        return !(isHPMax && isAmmoMax);
    }

    private void Action_Build()
    {
        var sentry = Instantiate(SentryPrefab);
        sentry.transform.position = TargetSentryBuild.transform.position;
        sentry.transform.rotation = TargetSentryBuild.transform.rotation;
        sentry.gameObject.SetActive(true);
        _timerToBuild = 2f;
        currentSentryGun = sentry;
        currentSentryGun.pivotObject.target = _mainGround;
        Hypatios.Player.Inventory.RemoveItem(itemMicrochip.GetID(), cost_metal);
        Hypatios.Player.Inventory.RemoveItem(itemRareMetal.GetID(), cost_raremetal);
    }

    private void Action_Control()
    {
        currentSentryGun.FireSentry();
    }

    private void Action_Destroy()
    {
        currentSentryGun.Stats.CurrentHitpoint = -1f;
        _timerToDestroy = 2f;
        isDestroyingHold = false;
    }


    private void Action_Repair()
    {
        _intervalRepair -= Time.deltaTime;

        if (_intervalRepair > 0) return;
        _intervalRepair = 1f / bulletPerSecond + 0.02f;

        currentSentryGun.sentryAmmo += perAmmoAmount;
        currentSentryGun.Stats.CurrentHitpoint += perHealAmount;

        if (currentSentryGun.sentryAmmo >= currentSentryGun.maxSentryAmmo)
            currentSentryGun.sentryAmmo = currentSentryGun.maxSentryAmmo;
        else curAmmo--;

        if (currentSentryGun.Stats.CurrentHitpoint >= currentSentryGun.Stats.MaxHitpoint.Value)
            currentSentryGun.Stats.CurrentHitpoint = currentSentryGun.Stats.MaxHitpoint.Value;
        else curAmmo--;

    }

    #endregion

    private void Audio_Processing(bool play)
    {
        if (play)
        {
            if (!audio_Processing.isPlaying)
                audio_Processing.Play();
        }
        else
        {
            if (audio_Processing.isPlaying)
                audio_Processing.Stop();
        }
    }
}
