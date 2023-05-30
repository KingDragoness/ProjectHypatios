using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public class CutsceneObject : MonoBehaviour
{

    public GameObject virtualCam;
    public List<GameObject> additionalVirtualCams; //primarily for cleanup
    public UnityEvent OnCutsceneEnded; 

    private List<CutsceneAction> allActionEntries = new List<CutsceneAction>();
    private List<CutsceneAction> currentActions = new List<CutsceneAction>();
    private bool isPlaying = false;
    CutsceneDialogueUI cutsceneUI;

    private void Start()
    {
        cutsceneUI = MainUI.Instance.cutsceneUI;
        allActionEntries = GetComponentsInChildren<CutsceneAction>().ToList();
        foreach (var v in additionalVirtualCams)
            v.gameObject.SetActive(false);
    }

    private float timer_continueWait = 0.5f;

    private void Update()
    {
        if (isPlaying == false)
        {
            return;
        }

        if (!cutsceneUI.allowContinue)
        {
            timer_continueWait -= Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.Return) && timer_continueWait < 0f)
            {
                cutsceneUI.SkipDialogue();
                cutsceneUI.allowContinue = true;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                NextActionEntry();
            }

            timer_continueWait = 0.5f;
        }
    }

    public void NextActionEntry()
    {
        if (currentActions.Count == 0)
        {
            StopCutscene();
            return;
        }

        currentActions[0].gameObject.SetActive(false);
        currentActions.RemoveAt(0);

        if (currentActions.Count > 0)
        {
            currentActions[0].ExecuteAction();
        }
    }

    [FoldoutGroup("Tools")]
    [Button("Next Action")]
    private void Debug_NextEntry()
    {
        NextActionEntry();
    }

    [FoldoutGroup("Tools")] [Button("Start Cutscene")]
    public void StartCutscene()
    {
        var MainUI1 = MainUI.Instance;
        MainUI1.current_UI = MainUI.UIMode.Cinematic;

        CloseAllCutsceneInstances();
        cutsceneUI.NewConversation();
        virtualCam.gameObject.SetActive(true);
        currentActions.AddRange(allActionEntries);
        currentActions[0].ExecuteAction();

        isPlaying = true;
    }

    [FoldoutGroup("Tools")] [Button("Stop All Cutscene")]
    private void StopCutscene()
    {
        var MainUI1 = MainUI.Instance;
        MainUI1.current_UI = MainUI.UIMode.Default;

        OnCutsceneEnded?.Invoke();

        CloseAllCutsceneInstances();
        isPlaying = false;
    }


    private void CloseAllCutsceneInstances()
    {
        List<CutsceneObject> allCutsceneObjects = FindObjectsOfType<CutsceneObject>().ToList();

        foreach (var cutscene in allCutsceneObjects)
        {
            cutscene.CloseCutscene();
        }
    }



    private void CloseCutscene()
    {
        virtualCam.gameObject.SetActive(false);
        additionalVirtualCams.RemoveAll(x => x == null);
        foreach (var go in allActionEntries)
            go.gameObject.SetActive(true);

        foreach (var vc in additionalVirtualCams)
            vc.gameObject.SetActive(false);
    }
}
