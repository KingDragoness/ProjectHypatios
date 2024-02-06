using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cinemachine;

public class CutsceneObject : MonoBehaviour
{

    public GameObject virtualCam;
    public List<GameObject> additionalVirtualCams; //primarily for cleanup
    public UnityEvent OnCutsceneEnded; 

    private List<CutsceneAction> allActionEntries = new List<CutsceneAction>();
    private List<CutsceneAction> currentActions = new List<CutsceneAction>();
    private bool isPlaying = false;
    CutsceneDialogueUI cutsceneUI;
    private CinemachineBlendDefinition.Style defaultBlendStyle;

    private List<CinemachineVirtualCamera> allVCs = new List<CinemachineVirtualCamera>();

    private void Awake()
    {
        allVCs = GetComponentsInChildren<CinemachineVirtualCamera>(true).ToList();
    }

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

        //Fuck Hypatios Cutscene
        foreach(var vc in allVCs)
        {
            vc.transform.position = Hypatios.MainCamera.transform.position;
            vc.transform.rotation = Hypatios.MainCamera.transform.rotation;
            vc.m_Lens.FieldOfView = Hypatios.MainCamera.fieldOfView;
            vc.m_Lens.Dutch = 0f;
            vc.LookAt = null;
            vc.Follow = null;
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
        MainUI1.OpenCinematic();
        defaultBlendStyle = cutsceneUI.cutsceneCamera.m_DefaultBlend.m_Style;

        CloseAllCutsceneInstances(this);
        cutsceneUI.NewConversation();
        virtualCam.gameObject.SetActive(true);
        currentActions.AddRange(allActionEntries);
        currentActions[0].ExecuteAction();

        isPlaying = true;
    }

    [FoldoutGroup("Tools")] [Button("Stop All Cutscene")]
    private void StopCutscene()
    {
        CloseAllCutsceneInstances();
        CloseCutsceneUI();
        OnCutsceneEnded?.Invoke();
    }


    [FoldoutGroup("Tools")]
    [Button("Copy all dialogue to clippy")]
    private void CopyDialogueToClipboard()
    {
        string s = "";
        var _dialogueEntries = GetComponentsInChildren<Cutscene_DialogEntry>().ToList();

        foreach (var dialogue in _dialogueEntries)
        {
            s += $"{dialogue.dialogSpeaker.name}: {dialogue.Dialogue_Content}\n";
        }

        GUIUtility.systemCopyBuffer = s;
    }


    public static void CloseAllCutsceneInstances(CutsceneObject ignoreCutscene = null)
    {
        List<CutsceneObject> allCutsceneObjects = FindObjectsOfType<CutsceneObject>().ToList();

        foreach (var cutscene in allCutsceneObjects)
        {
            if (cutscene == ignoreCutscene) continue;

            cutscene.CleanupAndClose();
        }
    }



    private void CloseCutsceneUI()
    {
        var MainUI1 = MainUI.Instance;
        MainUI1.CloseCinematic();


    }

    private void CleanupAndClose()
    {
        virtualCam.gameObject.SetActive(false);
        additionalVirtualCams.RemoveAll(x => x == null);
        foreach (var go in allActionEntries)
            go.gameObject.SetActive(true);

        foreach (var vc in additionalVirtualCams)
            vc.gameObject.SetActive(false);

        cutsceneUI.cutsceneCamera.m_DefaultBlend.m_Style = defaultBlendStyle;
        isPlaying = false;
    }
}
