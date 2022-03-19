using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StageChamberScript : MonoBehaviour
{

    public ChamberText chamberText;
    public GameObject sign_LevelStateCleared;
    public GameObject sign_LevelStateUnclear;
    public List<Enemy> enemiesToClear;
    public UnityEvent OnChamberCompleted;
    public AudioSource chamberAudioAnnouncement;
    public Animator anim;
    private bool cleared = false;

    public bool Cleared { get => cleared; set => cleared = value; }

    private void Start()
    {
        Enemy.onKilled += Enemy_onKilled;

        var anim_ = GetComponent<Animator>();

        if (anim_ != null)
        {
            anim = anim_;
        }
    }

    private void Enemy_onKilled(Enemy enemy)
    {
        enemiesToClear.Remove(enemy);
    }

    public void AddEnemy(Enemy enemy)
    {
        enemiesToClear.Add(enemy);
    }


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

    }

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
