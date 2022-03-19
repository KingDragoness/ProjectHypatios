using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SceneTransitionTrigger : MonoBehaviour
{

    public enum TransitionType
    {
        Additive,
        Specific
    }

    public TransitionType type = TransitionType.Specific;
    [HideIf("type", TransitionType.Additive)] public int targetLevel = 0;
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
        isLoading = true;
        if (type == TransitionType.Specific)
        {
            target = targetLevel;
        }
        else
        {
            target = Application.loadedLevel + additiveAmount;
        }

        FPSMainScript.instance.SaveGame(targetLevel: target);
        yield return new WaitForSeconds(2.2f);
        FPSMainScript.instance.BufferSaveData();



        Application.LoadLevel(target);

    }
}
