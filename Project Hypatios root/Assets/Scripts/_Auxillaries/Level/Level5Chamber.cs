using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Level5Chamber : MonoBehaviour
{
    public enum OperationType
    {
        Add,
        Subtract
    }

    [System.Serializable]
    public class Soal
    {

        public int a = 0;
        public int b = 0;
        public OperationType operationType;

        public bool IsAnswerCorrect(int answer)
        {

            if (operationType == OperationType.Add)
            {
                if (a + b == answer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (operationType == OperationType.Subtract)
            {
                if (a - b == answer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string GetSoal()
        {
            if (operationType == OperationType.Add)
            {
                return $"{a} + {b}";
            }
            else
            {
                return $"{a} - {b}";
            }
        }

    }

    public Soal currentSoal;
    [Space]
    public ChamberText chamberText; //display sisa soal2
    public ChamberText soalText; //display soalnya
    public ChamberText timerLeftText; //display sisa waktu
    public int sisaSoal = 15;
    public GameObject sign_LevelStateCleared;
    public GameObject sign_LevelStateUnclear;
    public Animator exitDoorAnim;
    public List<EnemyScript> enemiesToClear;
    public UnityEvent OnChamberCompleted;
    public UnityEvent OnChamberStarted;
    public UnityEvent OnAnswerWrong;
    public UnityEvent OnAnswerCorrect;

    [Space]
    [Header("Enemy Spawns")]
    public EnemyScript spiderEnemy;
    public Transform spawnTransform;
    public float range = 10;
    public int limitEnemy = 6;

    [Space]
    public AudioSource chamberAudioAnnouncement;
    public AudioSource chamberStartedAudio;
    public AudioSource jawabanBenarAudio;
    private bool cleared = false;

    public bool Cleared { get => cleared; set => cleared = value; }
    public bool HasStarted { get => hasStarted; }

    private bool hasStarted = false;
    private float timer = 15;
    private const float TIMER_LIMIT_ANSWER = 15;

    private void Start()
    {
        Hypatios.Enemy.OnEnemyDied += Enemy_onKilled;
    }

    private void OnDestroy()
    {
        Hypatios.Enemy.OnEnemyDied -= Enemy_onKilled;
    }

    public void StartChamber()
    {
        if (HasStarted)
        {
            return;
        }    

        hasStarted = true;
        //chamberStartedAudio.Play();
        DialogueSubtitleUI.instance.QueueDialogue("Attention to all facility users: The game of 'Calculator' has started.", "ANNOUNCER", 14f);
        timer = TIMER_LIMIT_ANSWER;
        NewSoal();
        SpawnSpider();
        OnChamberStarted?.Invoke();
    }

    private void Enemy_onKilled(EnemyScript enemy, DamageToken damagetoken)
    {
        enemiesToClear.Remove(enemy);
    }

    public void AddEnemy(EnemyScript enemy)
    {
        enemiesToClear.Add(enemy);
    }

    private void Update()
    {
        if (HasStarted == false)
        {
            return;
        }

        if (sisaSoal <= 0 && !Cleared)
        {
            ClearedChamber();
        }
        else if (!Cleared)
        {
            chamberText.SetTextContent(sisaSoal.ToString());
            soalText.SetTextContent(currentSoal.GetSoal());
            timerLeftText.SetTextContent((Mathf.RoundToInt(timer*10)/10).ToString() + "s");
            RunTest();
        }
    }

    private void RunTest()
    {
        if (enemiesToClear.Count > limitEnemy)
        {
            DialogueSubtitleUI.instance.QueueDialogue("Too many enemies in the chamber. Please clear them before continuing to the next question.", "ANNOUNCER", 3f);
            return;
        }

        if (timer < 0)
        {
            if (Time.timeScale <= 0 | sisaSoal <= 0)
            {
                return;
            }

            PenaltyTimeout();
            NewSoal();
            timer = TIMER_LIMIT_ANSWER + UnityEngine.Random.Range(-0, 2);
        }
        else
        {
            timer -= Time.unscaledDeltaTime;
        }
    }

    public void AnswerCheck(int answer)
    {
        if (currentSoal.IsAnswerCorrect(answer) == true)
        {
            NewSoal();
            jawabanBenarAudio.Play();
            DialogueSubtitleUI.instance.QueueDialogue("Correct! 7 soul rewarded... Onto the next question...", "ANNOUNCER", 5f);
            Hypatios.Game.SoulPoint += 7;
            OnAnswerCorrect?.Invoke();

        }
        else
        {
            MainGameHUDScript.Instance.audio_Error.Play();
            DialogueSubtitleUI.instance.QueueDialogue("Wrong answer!", "ANNOUNCER", 3f);
            OnAnswerWrong?.Invoke();

        }

    }

    private void PenaltyTimeout()
    {
        int amount = UnityEngine.Random.Range(2, 5);

        for(int x = 0; x < amount; x++)
        {
            SpawnSpider();
        }

        MainGameHUDScript.Instance.audio_Error.Play();
        DialogueSubtitleUI.instance.QueueDialogue("Timeout!", "ANNOUNCER", 3f);
    }

    private void NewSoal()
    {

        if (enemiesToClear.Count > limitEnemy)
        {
            DialogueSubtitleUI.instance.QueueDialogue("Too many enemies in the chamber. Please clear them before continuing to the next question.", "ANNOUNCER", 3f);
        }

        sisaSoal--;

        //generate new soal
        float chance = UnityEngine.Random.Range(0f, 1f);
        
        if (chance > 0.5f)
        {
            currentSoal.operationType = OperationType.Subtract;
        }
        else
        {
            currentSoal.operationType = OperationType.Add;
        }

        currentSoal.a = UnityEngine.Random.Range(10, 100);
        currentSoal.b = UnityEngine.Random.Range(0, currentSoal.a);

        timer = TIMER_LIMIT_ANSWER + UnityEngine.Random.Range(-1, 1);

        //just dont do fucking anything
        if (chance > 0.95f | chance < 0.05f)
        {
            //SpawnSpider();
        }
    }

    public void SpawnSpider()
    {
        Vector3 rangeSpawn = spawnTransform.position;
        rangeSpawn.x += UnityEngine.Random.Range(-range / 2, range / 2);
        rangeSpawn.z += UnityEngine.Random.Range(-range / 2, range / 2);

        var NewEnemy = Instantiate(spiderEnemy, transform);
        NewEnemy.transform.position = rangeSpawn;
        NewEnemy.gameObject.SetActive(true);
        AddEnemy(NewEnemy);
    }

    private void ClearedChamber()
    {
        exitDoorAnim.SetBool("IsOpened", true);
        sign_LevelStateCleared.gameObject.SetActive(true);
        sign_LevelStateUnclear.gameObject.SetActive(false);
        chamberText.SetTextContent(enemiesToClear.Count.ToString());

        if (!Cleared)
        {
            if (chamberAudioAnnouncement != null)
            {
                chamberAudioAnnouncement.Play();
                OnChamberCompleted?.Invoke();
                DialogueSubtitleUI.instance.QueueDialogue("Attention to all facility users: Chamber completed.", "ANNOUNCER", 14f);
            }
        }

        Cleared = true;
    }
}
