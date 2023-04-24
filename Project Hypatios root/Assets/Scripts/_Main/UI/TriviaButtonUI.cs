using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TriviaButtonUI : MonoBehaviour
{

    public TriviaUI triviaScript;
    public Image icon;
    public Sprite defaultSprite;
    public Text title;
    public Trivia trivia;
    public GameObject triviaCompleteIcon;
    public GameObject pivotStart;
    public GameObject pivotEnd;
    public Color UnlockedColor;
    public Color LockedColor;

    private void OnEnable()
    {
        Dehover();
    }

    public void RefreshTrivia()
    {
        if (Hypatios.Game.Check_TriviaCompleted(trivia))
        {
            triviaCompleteIcon.SetActive(true);
            title.text = trivia.Title;
            title.color = UnlockedColor;
            icon.sprite = trivia.SpriteIcon;
        }
        else
        {
            triviaCompleteIcon.SetActive(false);
            title.text = "???";
            title.color = LockedColor;
            icon.sprite = defaultSprite;
        }
    }

    public void Hover()
    {
        title.gameObject.SetActive(true);

        if (!Hypatios.Game.Check_TriviaCompleted(trivia))
        {
            return;
        }
        triviaScript.DescriptionHover(this);
    }

    public void Dehover()
    {
        title.gameObject.SetActive(false);
        triviaScript.Dehover();
    }

}
