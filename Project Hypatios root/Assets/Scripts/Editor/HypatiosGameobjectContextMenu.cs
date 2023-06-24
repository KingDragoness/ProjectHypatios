using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HypatiosGameobjectContextMenu : MonoBehaviour
{
    [MenuItem("GameObject/Hypatios/TP Player", false, -10)]
    public static void TeleportHere()
    {
        if (Application.isPlaying == false)
        {
            Debug.LogError("Please execute this only during runtime.");
            return;
        }

        Hypatios.Player.transform.position = Selection.activeGameObject.transform.position;
    }

    [MenuItem("GameObject/Hypatios/TP to Player", false, 1)]
    public static void TeleportObjectToPlayer()
    {
        if (Application.isPlaying == false)
        {
            Debug.LogError("Please execute this only during runtime.");
            return;
        }

        Selection.activeGameObject.transform.position = Hypatios.Player.transform.position;
    }
}
