using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TriviaShortButton : MonoBehaviour
{

    public TriviaMapUI triviaUI;
    public Trivia trivia;
    public Text labelName;
    public Image icon;

    public void Refresh()
    {
        labelName.text = trivia.Title;
    }

    public void ClickButton()
    {
        triviaUI.LookAtTriviaBall(this);
    }    

}
