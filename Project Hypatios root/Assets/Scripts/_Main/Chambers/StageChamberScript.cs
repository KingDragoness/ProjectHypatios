using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class StageChamberScript : MonoBehaviour
{



    public ChamberText chamberText;
    public GameObject sign_LevelStateCleared;
    public GameObject sign_LevelStateUnclear;
    public List<EnemyScript> enemiesToClear;
    public UnityEvent OnChamberCompleted;
    [FoldoutGroup("Stats")] [Tooltip("Enemy outside the volume will be killed instantly.")] public bool IsVolumeKillLimit = false;
    [FoldoutGroup("Stats")] [ShowIf("IsVolumeKillLimit")] public RandomSpawnArea VolumeKillBox;
    public AudioSource chamberAudioAnnouncement;
    public Animator anim;
    private bool cleared = false;

    public bool Cleared { get => cleared; set => cleared = value; }
    public static StageChamberScript Instance { get => _instance;  }

    private static StageChamberScript _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Hypatios.Enemy.OnEnemyDied += Enemy_onKilled;

        var anim_ = GetComponent<Animator>();

        if (anim_ != null)
        {
            anim = anim_;
        }
    }

    private void OnDestroy()
    {
        Hypatios.Enemy.OnEnemyDied -= Enemy_onKilled;
    }

    private void Enemy_onKilled(EnemyScript enemy, DamageToken damagetoken)
    {
        enemiesToClear.Remove(enemy);
    }

    public void AddEnemy(EnemyScript enemy)
    {
        enemiesToClear.Add(enemy);
    }

    private float _checkTimer = 2f;

    // Update is called once per frame
    void Update()
    {
        if (enemiesToClear.Count == 0 && !Cleared)
        {
            ClearedChamber();
        }
        else if (!Cleared)
        {
            chamberText.SetTextContent(enemiesToClear.Count.ToString());
        }

        if (_checkTimer >= 0f)
        {
            _checkTimer -= Time.deltaTime;
        }
        else
        {
            enemiesToClear.RemoveAll(x => x == null);
            if (IsVolumeKillLimit) CheckEnemyToKill();
            _checkTimer = 2f;
        }

    }

    private void CheckEnemyToKill()
    {
        DamageToken token = new DamageToken();
        token.damage = 9999f;

        foreach (var enemy in enemiesToClear)
        {
            if (VolumeKillBox.IsInsideOcclusionBox(enemy.OffsetedBoundWorldPosition) == false)
            {          
                enemy.Attacked(token);
            }
        }
    }

    [FoldoutGroup("Debug")]
    [Button("Complete Chamber")]
    private void ClearedChamber()
    {
        anim.SetBool("IsOpened", true);
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
