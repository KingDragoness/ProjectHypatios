using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DevLocker.Utils;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{

    public enum TransitionType
    {
        Additive,
        Specific
    }

    public TransitionType type = TransitionType.Specific;
    [HideIf("type", TransitionType.Additive)] public SceneReference scene;
    [ShowIf("type", TransitionType.Additive)] public int additiveAmount = 1;

    private bool isLoading = false;

    [ContextMenu("Trigger Scene")]
    public void SceneTrigger()
    {
        if (isLoading) return;

        MainGameHUDScript.Instance.FadeOutSceneTransition.gameObject.SetActive(true);
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        int target = 0;

        if (scene == null) yield break;

        isLoading = true;
        if (type == TransitionType.Specific)
        {
            target = scene.Index;
            Debug.Log($"{scene.SceneName} = ({scene.Index})");
        }
        else
        {
            target = Application.loadedLevel + additiveAmount;
        }

        if (Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.TutorialMode) Hypatios.Game.SaveGame(targetLevel: target);
        yield return new WaitForSeconds(2.2f);
        if (Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.TutorialMode) Hypatios.Game.BufferSaveData();



        Application.LoadLevel(target);

    }
}
