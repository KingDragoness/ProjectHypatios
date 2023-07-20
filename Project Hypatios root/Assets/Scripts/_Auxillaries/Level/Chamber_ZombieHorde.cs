using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Chamber_ZombieHorde : MonoBehaviour
{

    public enum Stage
    {
        NotStart,
        Warmup,
        Started,
        Finished
    }

    [FoldoutGroup("UI")] public Text label_TimeRound;
    [FoldoutGroup("UI")] public Slider slider_TimeCount;
    [FoldoutGroup("References")] public AudioSource audio_CompletedChamber;
    [FoldoutGroup("Parameters")] public float RoundTimer = 600f;
    [FoldoutGroup("Parameters")] public float SpawnTimeRefresh = 4f;
    [FoldoutGroup("Parameters")] public int zombieLimit = 70;
    [FoldoutGroup("Parameters")] public int spiderLimit = 10;
    [FoldoutGroup("Parameters")] [Range(0f,1f)] public float chanceSpawnSpider = 0.2f;
    public UnityEvent OnRoundStarted;
    public UnityEvent OnFinished;
    public InstantiateRandomObject randomSpawner;
    public InstantiateRandomObject spiderSpawner;
    public Stage currentStage = Stage.NotStart;

    private float _timerSpawn = 4f;
    private bool _hasFinished = false;
    private float _startingTimer = 0f;

    private void Start()
    {
        _startingTimer = RoundTimer;
    }

    private void Update()
    {
        UpdateTimeUI();

        if (currentStage == Stage.NotStart)
            return;

        HandleUpdate();

        if (currentStage == Stage.Started)
        {
            _timerSpawn -= Time.deltaTime;

            if (_timerSpawn <= 0)
            {
                float chance = UnityEngine.Random.Range(0f, 1f);
                if (chance > chanceSpawnSpider)
                    SpawnZombies();
                else
                    SpawnSpider();
                _timerSpawn = SpawnTimeRefresh;
            }
        }
    }

    public void HandleUpdate()
    {
        if (RoundTimer <= 0)
            currentStage = Stage.Finished;
        else
            RoundTimer -= Time.deltaTime;

        if (currentStage == Stage.Finished && _hasFinished == false)
        {
            Finished();
        }
    }

    private void Finished()
    {
        if (audio_CompletedChamber != null)
        {
            audio_CompletedChamber.Play();
            OnFinished?.Invoke();
            Hypatios.Dialogue.QueueDialogue("Attention to all facility users: Chamber completed.", "ANNOUNCER", 14f);
        }

        _hasFinished = true;
    }

    public void StartZombieLevel()
    {
        if (currentStage == Stage.NotStart)
        {
            OnRoundStarted?.Invoke();
            DeadDialogue.PromptNotifyMessage_Mod("The horde are coming.", 4f);
        }

        currentStage = Stage.Started;
    }

    private void UpdateTimeUI()
    {
        var rt = RoundTimer;
        if (rt <= 0) rt = 0;
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(rt, true);
        label_TimeRound.text = $"{ dateTime.Minute.ToString("00")}:{ dateTime.Second.ToString("00")}";
        slider_TimeCount.value = RoundTimer;
        slider_TimeCount.maxValue = _startingTimer;
    }

    private void SpawnZombies()
    {
        if (Monster_ZombieMobius.TotalZombieInScene > zombieLimit)
            return;
        randomSpawner.SpawnThing();
    }

    private void SpawnSpider()
    {
        var spiderEnemy = spiderSpawner.prefabs[0].GetComponent<EnemyScript>();
        int totalSpider = Hypatios.Enemy.CountEnemyOfType(spiderEnemy);

        if (totalSpider < spiderLimit)
            spiderSpawner.SpawnThing();
    }
}
