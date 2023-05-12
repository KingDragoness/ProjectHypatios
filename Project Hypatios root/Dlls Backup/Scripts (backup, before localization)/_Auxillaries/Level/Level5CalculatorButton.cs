using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level5CalculatorButton : MonoBehaviour
{

    public TextMesh textMesh;
    public string operationName = "";
    public Level5CalculatorLayout calculatorScript;

    public void ButtonSelected()
    {
        calculatorScript.InputOperation(operationName);
    }

}
