using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// In order to help gauge the size of mesh objects,
/// this script displays the dimensions in world space of a selected object 
/// with a MeshRenderer attached whenever it's selected.
/// </summary>
public class MeshMeasure : Editor
{
    protected virtual void OnSceneGUI()
    {
        MeshRenderer meshRenderer = (MeshRenderer)target;

        if (meshRenderer == null)
        {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;

        Vector3 position = meshRenderer.transform.position + Vector3.up * 2f;
        Handles.Label(position, meshRenderer.bounds.size.ToString(), style);
    }
}