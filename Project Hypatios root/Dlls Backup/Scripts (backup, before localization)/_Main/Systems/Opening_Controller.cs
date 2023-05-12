using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using DevLocker.Utils;

public class Opening_Controller : MonoBehaviour
{
    public int framerateTarget = 31;
    public float targetTimeSkip = 70f;
    public PlayableDirector directorScript;
    public SceneReference mainMenuScene;

    private void Start()
    {
        Application.targetFrameRate = framerateTarget;
    }

    public void InstantSkipScene()
    {
        if (directorScript.time < targetTimeSkip)
            directorScript.time = targetTimeSkip;
    }

    public void Menu_SkipMenu()
    {
        int index = mainMenuScene.Index;
        Application.LoadLevel(index);
    }


}
