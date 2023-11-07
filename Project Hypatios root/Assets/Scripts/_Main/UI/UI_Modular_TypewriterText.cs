using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UI_Modular_TypewriterText : MonoBehaviour
{

    public Text text_DialogueContent;
    public TextMesh text_TextMesh;
    public float secondsPerChar = 0.1f;
    public bool textInteruptableByDisable = true;
    [TextArea(2,10)] public string dialogText;

    private IEnumerator currentCoroutine;
    private bool disabledCache = false;
    private string _cachedText = "";

    private void OnEnable()
    {
        if (disabledCache)
            TypeThisDialogue(dialogText);

        if (textInteruptableByDisable == false)
        {
            currentCoroutine = Typewriter(_cachedText);
            StartCoroutine(currentCoroutine);
        }
    }

    public void TypeThisDialogue(string text)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        if (gameObject.activeInHierarchy == false)
        {
            dialogText = text;
            disabledCache = true;
            return;
        }

        currentCoroutine = Typewriter(text);
        _cachedText = text;
        StartCoroutine(currentCoroutine);
    }

    public void TypeThisDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        if (gameObject.activeInHierarchy == false)
        {
            disabledCache = true;
            return;
        }

        currentCoroutine = Typewriter(dialogText);
        _cachedText = dialogText;
        StartCoroutine(currentCoroutine);
    }

    IEnumerator Typewriter(string text)
    {
        if (text_DialogueContent != null) text_DialogueContent.text = "";
        if (text_TextMesh != null) text_TextMesh.text = "";
        disabledCache = false;

        var waitTimer = new WaitForSeconds(secondsPerChar);
        foreach (char c in text)
        {
            if (text_DialogueContent != null) text_DialogueContent.text = text_DialogueContent.text + c;
            if (text_TextMesh != null) text_TextMesh.text = text_TextMesh.text + c;
            yield return waitTimer;
        }

    }
}
