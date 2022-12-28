using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPrintConsole : MonoBehaviour
{

    public string output = "test1";

    public void Print()
    {
        Debug.Log(output);
    }

}
