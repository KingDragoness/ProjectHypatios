using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Video;
using Sirenix.OdinInspector;

public class Level1_IntroScript : MonoBehaviour
{

    public GameObject playerMain;
    public GameObject specialIntro;
    public GameObject UI_FadeIn;
    public GameObject UI_FadeOut;

    [Header("First Run")]
    public VideoPlayer videoPlayer;

    [Header("Other run")]
    public GameObject cineBrain;
    public GameObject fakePlayer;
    public GameObject triggererName;

    [Space]
    public UnityEvent OnIntroEnded;
    public UnityEvent OnIntroStarted;

    private bool hasStarted = false;

    private void Awake()
    {
        OverrideParameters();
    }

    private void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(StartIntro());
    }

    private void OverrideParameters()
    {
        if (FPSMainScript.savedata != null)
        {
            Hypatios.Game.LoadGameFromKilled();
        }
    }

    private bool firstRun = false;

    IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(.2f);
        playerMain.gameObject.SetActive(false);
        specialIntro.gameObject.SetActive(true);
        UI_FadeIn.gameObject.SetActive(false);
        //UI_FadeOut.gameObject.SetActive(true);

        if (Hypatios.Game.TotalRuns == 0 && Application.isEditor == false)
        {
            videoPlayer.gameObject.SetActive(true);
            cineBrain.gameObject.SetActive(false);
            fakePlayer.gameObject.SetActive(false);
            firstRun = true;
            OnIntroStarted?.Invoke();
        }
        else
        {
            videoPlayer.gameObject.SetActive(false);
            cineBrain.gameObject.SetActive(true);
            fakePlayer.gameObject.SetActive(true);
            OnIntroEnded?.Invoke();
        }
        //mainGame_CanvasGroup.alpha = 0;
    }

    [Button("ForceStartCinematic")]
    public void StartIntroForce()
    {
        playerMain.gameObject.SetActive(false);
        specialIntro.gameObject.SetActive(true);
        UI_FadeIn.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(true);
        cineBrain.gameObject.SetActive(false);
        fakePlayer.gameObject.SetActive(false);
        firstRun = true;
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > 3)
        {
            if (videoPlayer.isPlaying == false && !hasStarted && firstRun)
            {
                StartRealgame();
            }
        }
    }

    [ContextMenu("StartRealgame")]
    public void StartRealgame()
    {
        playerMain.gameObject.SetActive(true);
        specialIntro.gameObject.SetActive(false);
        UI_FadeIn.gameObject.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
        playerMain.transform.eulerAngles = fakePlayer.transform.eulerAngles;
        playerMain.transform.position = fakePlayer.transform.position;
        //mainGame_CanvasGroup.alpha = 1;
        OnIntroEnded?.Invoke();
        hasStarted = true;

        if (Hypatios.Game.TotalRuns == 0)
        {
            triggererName.gameObject.SetActive(true);
        }
        else
        {
            triggererName.gameObject.SetActive(false);
        }

    }

    [SerializeField] private bool hasTriggeredPlayerNaming = false;

    public void EnterPlayerName()
    {
        if (Hypatios.Game.TotalRuns == 0) 
        { 
            MainGameHUDScript.Instance.ShowPromptUI("", "Enter your name:", true);
            MainUI.Instance.SetTempoPause(true);
            hasTriggeredPlayerNaming = true;
        }
    }

}
