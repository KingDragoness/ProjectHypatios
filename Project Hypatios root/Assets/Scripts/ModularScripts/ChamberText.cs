using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberText : MonoBehaviour
{

    public TextMesh textMesh;

    void Start()
    {
        
    }

    public void SetTextContent(string s)
    {
        textMesh.text = s;
    }
}
