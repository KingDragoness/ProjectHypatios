using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class HackingDeviceWeapon : GunScript
{

    [System.Serializable]
    public class EnemyIcons
    {
        public BaseEnemyStats baseEnemyStats;
        public Material monitorMaterial;
    }

    public enum Mode
    {
        Scan,
        PrepareHack,
        Hacking
    }

    [FoldoutGroup("Hacking Device")] public EnemyScript currentEnemyScript;
    [FoldoutGroup("Hacking Device")] public TextMesh DigitOutputLabel;
    [FoldoutGroup("Hacking Device")] public TextMesh TargetEnemyLabel;
    [FoldoutGroup("Hacking Device")] public TextMesh AmmoLabel;
    [FoldoutGroup("Hacking Device")] public Mode currentState;
    [FoldoutGroup("Hacking Device")] public MeshRenderer Monitor_meshRendr;
    [FoldoutGroup("Hacking Device")] public LineRenderer targetLineRendr;
    [FoldoutGroup("Hacking Device")] public Transform outLine;
    [FoldoutGroup("Hacking Device")] public List<EnemyIcons> AllEnemyIcons = new List<EnemyIcons>();
    [FoldoutGroup("Hacking Device")] public List<GameObject> AllSignalLevelIcons = new List<GameObject>();
    [FoldoutGroup("Audios")] public AudioSource audio_Hacking;
    [FoldoutGroup("Audios")] public AudioSource audio_HackingSuccess;
    [FoldoutGroup("Audios")] public AudioSource audio_HackingFailed;

    private float prepareHackTime = 1.3f;
    private float _preparingHackTimer = 0f;
    private float _hackingTimer = 0f;
    private float hackingTime = 4f;
    private const float hackingTimeBonus = 0.5f; //Always adds by 1
    private float _timerDisplayTempText = 3f;

    #region Update States
    public override void Update()
    {
        base.Update();

        if (Time.timeScale <= 0)
        {
            return;
        }

        _timerDisplayTempText -= Time.deltaTime;

        float percent = Mathf.Floor(((float)curAmmo/(float)magazineSize) * 100f);
        AmmoLabel.text = $"{percent}%";

        if (currentEnemyScript != null)
        {
            float currentDist = Vector3.Distance(transform.position, currentEnemyScript.transform.position);
            float signalRange = GetFinalValue("SignalRange");

            if (signalRange < currentDist)
            {
                currentEnemyScript = null;

                if (currentState == Mode.PrepareHack | currentState == Mode.Hacking)
                {
                    LostSignal();
                }
              
            }


            if (isReloading && (currentState == Mode.PrepareHack | currentState == Mode.Hacking))
            {
                ResetToScanMode();
            }
        }


        if (currentState == Mode.PrepareHack)
            PrepareHackMode();

        if (currentState == Mode.Scan && !isReloading)
            ScanMode();

        if (currentState == Mode.Hacking)
        {
            HackingMode();
            if (!audio_Hacking.isPlaying) audio_Hacking.Play();
        }
        else
        {
            if (audio_Hacking.isPlaying) audio_Hacking.Stop();
        }

        DisplayMonitorEnemy();

    }
    private void ResetToScanMode()
    {
        currentState = Mode.Scan;
        currentEnemyScript = null;
        _preparingHackTimer = 0f;
    }

    private void ScanMode()
    {
        ScanAnyEnemies();
        if (_timerDisplayTempText <= 0) DigitOutputLabel.text = "0000.0000.0000";
    }

    string pregen_MechPasscode = "0000.0000.0000";

    private void PrepareHackMode()
    {
        _preparingHackTimer -= Time.deltaTime;
        string s = "0000.0000.0000";


        if (_preparingHackTimer <= 0f)
        {
            currentState = Mode.Hacking;
            hackingTime = GetFinalValue("HackingTime");
            _hackingTimer = hackingTime + hackingTimeBonus;
            GenerateAIPasscode();
        }

        if (_timerDisplayTempText <= 0) DigitOutputLabel.text = $"{s}";

    }

    private void GenerateAIPasscode()
    {
        const string glyphs = "0123456789"; 
        string s = "";

        int charAmount = Random.Range(14, 14); 
        for (int i = 0; i < charAmount; i++)
        {
            int t1 = i + 1;
            if (t1 % 5 == 0)
            {
                s += ".";
                continue;
            }
            s += glyphs[Random.Range(0, glyphs.Length)];
        }

        pregen_MechPasscode = s;
    }

    public void DisplayTempText(string _label, float _timer)
    {
        DigitOutputLabel.text = $"{_label}";
        _timerDisplayTempText = _timer;
    }

    private void LostSignal()
    {
        DisplayTempText("LOST.TARG.ET_!", 2f);
        audio_HackingFailed.Play();
        ResetToScanMode();
    }

    private void HackingMode()
    {
        if (curAmmo <= 0)
        {
            DigitOutputLabel.gameObject.SetActive(true);
            DisplayTempText("NO_B.ATTE.RY_!", 2f);
            audio_HackingFailed.Play();
            ResetToScanMode();
            return;
        }

        _hackingTimer -= Time.deltaTime;
        const string glyphs = "0123456789"; //add the characters you want
        int charAmount = Random.Range(14, 14); //set those to the minimum and maximum length of your string
        int numerals = 12;
        float timeEachNumeral = hackingTime / (float)numerals;
        string s = "";

        for (int i = 0; i < charAmount; i++)
        {
            int t1 = i + 1;
            if (t1 % 5 == 0)
            {
                s += ".";
                continue;
            }

            float _timeOfi = timeEachNumeral * (float)i;

            if ((_hackingTimer - hackingTimeBonus) > _timeOfi)
                s += glyphs[Random.Range(0, glyphs.Length)];
            else
                s += pregen_MechPasscode[i];
        }

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / bulletPerSecond + 0.05f;
            curAmmo--;
        }

        if (currentEnemyScript == null)
        {
            LostSignal();
            return;
        }
        else if (currentEnemyScript.Stats.IsDead)
        {
            LostSignal();
            return;
        }

        DigitOutputLabel.text = $"{s}";

        if (_hackingTimer < hackingTimeBonus)
        {
            DigitOutputLabel.text = $"SUCC.ESS_.___!";
            if (audio_HackingSuccess.isPlaying == false) audio_HackingSuccess.Play();

            if (Mathf.FloorToInt(Time.time * 11) % 2 == 0)
                DigitOutputLabel.gameObject.SetActive(true);
            else 
                DigitOutputLabel.gameObject.SetActive(false);

        }

        if (_hackingTimer <= 0f)
        {
            float restoreLv = GetFinalValue("Restore");

            if (restoreLv >= 1)
                currentEnemyScript.Stats.CurrentHitpoint = currentEnemyScript.Stats.MaxHitpoint.Value;

            //success!
            currentEnemyScript.Hack();
            DigitOutputLabel.gameObject.SetActive(true);
            ResetToScanMode();
        }
    }


    #endregion


    private void ScanAnyEnemies()
    {
        float signalRange = GetFinalValue("SignalRange");

        var enemyScript = Hypatios.Enemy.FindEnemyEntity(Alliance.Player, UnitType.Mechanical, myPos: transform.position, maxDistance: signalRange);

        if (enemyScript != null)
            currentEnemyScript = enemyScript as EnemyScript;
    }

    public override void FireInput()
    {
        //base.FireInput();

        if (Hypatios.Input.Fire1.WasReleasedThisFrame() && curAmmo > 0  && !isReloading && IsRecentlyPaused()
            && currentEnemyScript != null && currentState == Mode.Scan)
        {
            anim.SetTrigger("Fire");
            _preparingHackTimer = prepareHackTime;
            currentState = Mode.PrepareHack;
            ConsumeAmmo(Mathf.FloorToInt(bulletPerSecond));
        }

    }

   
    private void ConsumeAmmo(int ammo)
    {
        curAmmo -= ammo;

        if (curAmmo < 0f)
            curAmmo = 0;
    }

    private void DisplayMonitorEnemy()
    {
        if (currentEnemyScript == null)
        {
            OffMonitor();
            return;
        }

        EnemyIcons enemyIcon = AllEnemyIcons.Find(x => x.baseEnemyStats.name == currentEnemyScript.EnemyName);

        if (enemyIcon == null)
        {
        }
        else
             {
            Monitor_meshRendr.material = enemyIcon.monitorMaterial;

        }


        Monitor_meshRendr.gameObject.SetActive(true);
        targetLineRendr.gameObject.SetActive(true);
        TargetEnemyLabel.text = $"TARGET: {currentEnemyScript.EnemyName}";
        Vector3[] v3 = new Vector3[2];
        v3[0] = outLine.transform.position;
        v3[1] = currentEnemyScript.transform.position;
        targetLineRendr.SetPositions(v3);

        float signalRange = GetFinalValue("SignalRange");
        float dist = Vector3.Distance(transform.position, currentEnemyScript.transform.position);

        //4 signal bars (range: 10m)
        //0 - 2.5 = 3
        //2.5 - 5 = 2
        //5 - 7.5 = 1
        //7.5 - 10 = 0

        for(int x = 0; x < AllSignalLevelIcons.Count; x++)
        {
            int count1 = AllSignalLevelIcons.Count;
            float perQuadrantValue = signalRange / (float)count1;
            bool withinThisQuadrant = false;

            float highThreshold = (x+1) * perQuadrantValue;
            float lowThreshold = (x) * perQuadrantValue;

            if (x == 0)
                withinThisQuadrant = true;

            if ((signalRange - dist) > lowThreshold)
                withinThisQuadrant = true;

            if (withinThisQuadrant)
            {
                AllSignalLevelIcons[x].gameObject.SetActive(true);
            }
            else
            {
                AllSignalLevelIcons[x].gameObject.SetActive(false);
            }

        }
    }

    private void OffMonitor()
    {
        for (int x = 0; x < AllSignalLevelIcons.Count; x++)
        {
            AllSignalLevelIcons[x].gameObject.SetActive(false);
        }

        Monitor_meshRendr.gameObject.SetActive(false);
        targetLineRendr.gameObject.SetActive(false);
        TargetEnemyLabel.text = "NO NEARBY MECHS";
    }
}
