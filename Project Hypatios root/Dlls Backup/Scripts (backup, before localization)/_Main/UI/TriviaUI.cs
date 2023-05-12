using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;

public class TriviaUI : MonoBehaviour
{

    public TriviaButtonUI buttonUI;
    public UILineRenderer baseLineRender;
    public RectTransform parentTrivias;
    public Transform parentLineRenders;
    public Text descript_Title;
    public Text descript_Description;


    public List<TriviaButtonUI> allTriviaButtons = new List<TriviaButtonUI>();
    public List<UILineRenderer> allLineRenderers = new List<UILineRenderer>();

    private void OnEnable()
    {
        UpdateTrivia();
    }

    public void UpdateTrivia()
    {

        var allButtons = parentTrivias.GetComponentsInChildren<TriviaButtonUI>();
        allTriviaButtons = allButtons.ToList();

        foreach (var button in allTriviaButtons)
        {
            button.RefreshTrivia();
        }
        GenerateTriviaLine();

    }


    [Button("Editor - Update trivia menu")]
    public void UpdateEditorTrivia()
    {

        var allButtons = parentTrivias.GetComponentsInChildren<TriviaButtonUI>();
        allTriviaButtons = allButtons.ToList();

        GenerateTriviaLine();
    }

    private void GenerateTriviaLine()
    {
        foreach(var linerend in allLineRenderers)
        {
            if (linerend == null) continue;
            DestroyImmediate(linerend.gameObject);
        }
        allLineRenderers.RemoveAll(t => t == null);
        var allTriviasWithRequirements = allTriviaButtons.FindAll(x => x.trivia.previousTrivia != null);

        foreach(var triviabutton1 in allTriviasWithRequirements)
        {
            var newLine = Instantiate(baseLineRender, parentLineRenders);
            newLine.Points = new Vector2[2];
            var buttonPrev = FindButtonByPreviousTrivia(triviabutton1.trivia);
            var startPoint = buttonPrev.pivotEnd.GetComponent<RectTransform>().localPosition + buttonPrev.GetComponent<RectTransform>().localPosition; // hard coded
            var endPoint = triviabutton1.pivotStart.GetComponent<RectTransform>().localPosition + triviabutton1.GetComponent<RectTransform>().localPosition; //goddamn hard coded


            {
                //startPoint += baseLineRender.transform.localPosition;
                //endPoint += baseLineRender.transform.localPosition;
            }
   

            newLine.Points[0] = startPoint;
            newLine.Points[1] = endPoint;
            newLine.gameObject.SetActive(true);

            allLineRenderers.Add(newLine);
        }
    }

    private TriviaButtonUI FindButtonByPreviousTrivia(Trivia currentTrivia)
    {
        return allTriviaButtons.Find(x => x.trivia == currentTrivia.previousTrivia);
    }

    public void DescriptionHover(TriviaButtonUI _buttonUI)
    {
        descript_Title.text = _buttonUI.trivia.Title;
        descript_Description.text = _buttonUI.trivia.Description;
    }

    public void Dehover()
    {
        descript_Title.text = "";
        descript_Description.text = "";
    }

}
