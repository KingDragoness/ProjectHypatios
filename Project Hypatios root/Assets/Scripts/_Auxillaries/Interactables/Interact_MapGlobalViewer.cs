using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_MapGlobalViewer : MonoBehaviour
{

    public TextMesh textmesh;

    private void Start()
    {
        var chamberObj = Hypatios.Chamber.chamberObject;

        if (chamberObj != null)
        {
            textmesh.text = $"{chamberObj.TitleCard_Title}";
        }

    }

}
