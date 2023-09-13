using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_NewsPrompter : MonoBehaviour
{

    public Text label_NewsPrompt;
    public static MainMenu_NewsPrompter instance;

    private void Awake()
    {
        instance = this;
    }

    public void InsertText(string text)
    {
        label_NewsPrompt.text = $"{text}";
    }

}
