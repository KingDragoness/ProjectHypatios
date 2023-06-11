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
}
