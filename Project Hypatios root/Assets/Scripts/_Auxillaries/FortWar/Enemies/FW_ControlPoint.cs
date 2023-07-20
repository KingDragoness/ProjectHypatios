using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class FW_ControlPoint : MonoBehaviour
{

    /// <summary>
    /// 0 = Finalbase!
    /// 1 = CP1
    /// 2 = CP2
    /// </summary>
    public int CPNumber = 1;
    public RandomSpawnArea areaCP;
    public bool isCaptured = false;
    public UnityEvent OnCaptured;
    public AudioSource audio_CPCaptured;
    public GameObject CapturingGO;

    [ProgressBar(0,1f)]
    public float captureProgress = 0f;

    private int _currentInvadersInArea = 0;
    private CharacterScript characterScript;

    private void Start()
    {
        characterScript = FindObjectOfType<CharacterScript>();
    }

    public int CurrentInvadersInArea
    {
        get { return _currentInvadersInArea; }
    }

    private void Update()
    {
        float time = Mathf.FloorToInt(Time.time*10);

        if (time % 2 == 0)
        {
            return;
        }

        var listInvaders = Chamber_Level7.instance.AllUnits.Where(x => x.Alliance == FW_Alliance.INVADER);
        List<Transform> allUnits = new List<Transform>();

        foreach (var unit in listInvaders)
        {
            allUnits.Add(unit.transform);
        }

        allUnits.Add(characterScript.transform);

        int i = 0;

        foreach (var invader in allUnits)
        {
            if (areaCP.IsInsideOcclusionBox(invader.transform.position))
            {
                i++;
            }
        }

        _currentInvadersInArea = i;
        SetCaptureProgress();
    }

    private const float _speed1 = 0.035f;
    private const float _speed2 = 0.05f;
    private const float _speed3 = 0.07f;
    private const float _speed4 = 0.09f;
    private bool isContested = false;

    private void SetCaptureProgress()
    {
        float _captureSpeed = 0.02f;


        if (_currentInvadersInArea == 1) _captureSpeed = _speed1;
        if (_currentInvadersInArea == 2) _captureSpeed = _speed2;
        if (_currentInvadersInArea == 3) _captureSpeed = _speed3;
        if (_currentInvadersInArea == 4) _captureSpeed = _speed4;
        if (_currentInvadersInArea >= 5) _captureSpeed = 0.15f;
        if (_currentInvadersInArea >= 8) _captureSpeed = 0.5f;

        if (_currentInvadersInArea > 0)
        {
            captureProgress += Time.deltaTime * _captureSpeed;
            isContested = true;
        }
        else if (!isCaptured)
        {
            captureProgress -= Time.deltaTime * 0.08f;
            isContested = false;
        }


        captureProgress = Mathf.Clamp(captureProgress, 0f, 1f);

        if (isCaptured)
        {
            captureProgress = 1f;
            isContested = false;
        }

        if (isContested)
            Contested();
        else
            NotContested();

        if (captureProgress >= 1f && !isCaptured)
        {
            CaptureCP();
        }
    }

    private void Contested()
    {
        if (CapturingGO.gameObject.activeSelf == true) return;
        CapturingGO.gameObject.SetActive(true);
    }

    private void NotContested()
    {
        if (CapturingGO.gameObject.activeSelf == false) return;
        CapturingGO.gameObject.SetActive(false);

    }

    public void CaptureCP(bool playAudio = true)
    {
        isCaptured = true;
        OnCaptured?.Invoke();

        if (audio_CPCaptured != null && CPNumber != 0)
        {
            if (playAudio) audio_CPCaptured.Play();
            Hypatios.Dialogue.QueueDialogue("Attention, a control point has been captured by the red team.", "ANNOUNCER", 8f);
        }
    }

}
