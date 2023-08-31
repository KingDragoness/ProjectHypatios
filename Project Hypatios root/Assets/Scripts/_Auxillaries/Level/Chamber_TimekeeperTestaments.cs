using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DevLocker.Utils;

public class Chamber_TimekeeperTestaments : MonoBehaviour
{

    public SceneReference darkDreamScene;
    public UnityEvent OnStartTransition;

    private bool hasStarted = false;
    private float _timer = 0f;
    private bool hasTriggered = false;

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        if (hasStarted == false) return;
        _timer -= Time.deltaTime;

        if (hasTriggered == false && _timer < 0f)
        {
            GoToDarkDreams();
        }
    }

    public void StartTransition(float time = 1f)
    {
        OnStartTransition?.Invoke();
        _timer = time;
        hasStarted = true;
    }

    public void GoToDarkDreams()
    {
        Hypatios.Game.PlayerDie();
        Application.LoadLevel(darkDreamScene.Index);
        hasTriggered = true;
    }
}
