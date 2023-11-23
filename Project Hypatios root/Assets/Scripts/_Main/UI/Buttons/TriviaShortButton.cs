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
    public CodexHintTipsSO codexSO;
    public ButtonType type = ButtonType.Trivia;
    public Text labelName;
    public Image icon;

    public enum ButtonType
    {
        Flag,
        Trivia,
        Codex
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
        else if (type == ButtonType.Codex)
        {
            labelName.text = codexSO.Title;
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
        else if (type == ButtonType.Codex)
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
