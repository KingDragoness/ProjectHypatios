using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TriviaShortButton : MonoBehaviour
{

    public TriviaMapUI triviaUI;
    public Trivia trivia;
    public GlobalFlagSO flagSO;
    public ButtonType type = ButtonType.Trivia;
    public Text labelName;
    public Image icon;

    public enum ButtonType
    {
        Flag,
        Trivia
    }

    public void Refresh()
    {
        if (type == ButtonType.Trivia)
        {
            labelName.text = trivia.Title;
        }
        else if (type == ButtonType.Flag)
        {
            labelName.text = flagSO.DisplayName;
        }
    }

    public void ClickButton()
    {
        if (type == ButtonType.Trivia)
        {
            triviaUI.LookAtTriviaBall(this);
        }
    }

    public void Highlight()
    {

        if (type == ButtonType.Flag)
        {
            triviaUI.HighlightWindow(this);

        }
        else
        {
            triviaUI.LookAtTriviaBall(this);

        }

    }

    public void Dehighlight()
    {

        if (type == ButtonType.Flag)
        {
            triviaUI.DehighlightWindow(this);
        }

    }
}
