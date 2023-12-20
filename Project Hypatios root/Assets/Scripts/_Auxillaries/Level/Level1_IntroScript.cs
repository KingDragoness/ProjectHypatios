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
    public GameObject UI_ForceSkip;

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
    private bool _playerGainedHUD = false;

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
        UI_ForceSkip.gameObject.SetActive(false);

        //UI_FadeOut.gameObject.SetActive(true);

        //never play the damn intro cutscene again
        if (Hypatios.Game.TotalRuns == 9999999 && Application.isEditor == false)
        {
            StartCinematic();
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
        StartCinematic();
        firstRun = true;
    }

    public void StartCinematic()
    {
        Cursor.lockState = CursorLockMode.None;
        videoPlayer.gameObject.SetActive(true);
        cineBrain.gameObject.SetActive(false);
        fakePlayer.gameObject.SetActive(false);
        UI_ForceSkip.gameObject.SetActive(true);
        firstRun = true;
        OnIntroStarted?.Invoke();
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > 3 && hasStarted == false)
        {
            if (videoPlayer.isPlaying == false && !hasStarted && firstRun)
            {
                StartRealgame();
            }

            if (videoPlayer.isPlaying == true)
            {
                Hypatios.UI.SetPauseState(true);
            }
            else
            {
                Hypatios.UI.SetPauseState(false);
            }
        }

        if (_playerGainedHUD == false && videoPlayer.isPlaying == false)
        {
            Hypatios.UI.SetPauseState(false);
        }

    }

    public void GainHUD()
    {
        _playerGainedHUD = true;
    }

    [ContextMenu("StartRealgame")]
    public void StartRealgame()
    {
        playerMain.gameObject.SetActive(true);
        specialIntro.gameObject.SetActive(false);
        UI_FadeIn.gameObject.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
        UI_ForceSkip.gameObject.SetActive(false);
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

        Hypatios.UI.SetPauseState(false);

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
