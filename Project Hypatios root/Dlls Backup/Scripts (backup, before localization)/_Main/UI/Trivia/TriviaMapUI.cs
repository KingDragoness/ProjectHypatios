using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TriviaMapUI : MonoBehaviour
{

    public TriviaBallButton currentHoveredBall;
    [Space]
    public TriviaBallButton triviaBall;
    public Wire wire;
    public Transform farlandPoint;
    [FoldoutGroup("Preview")] public GameObject previewDescriptionPanel;
    [FoldoutGroup("Preview")] public Text titleLabel;
    [FoldoutGroup("Preview")] public Text descriptionLabel;
    [FoldoutGroup("Preview")] public Image image;
    [FoldoutGroup("Preview")] public Material tvNoiseMat;
    [FoldoutGroup("Preview")] public Sprite cut1BeginningSprite;
    public Transform parentTrivias;
    public Transform parentLineRenders;

    public List<TriviaBallButton> allTriviaButtons = new List<TriviaBallButton>();
    public List<Wire> allLineRenderers = new List<Wire>();

    private void OnEnable()
    {
        UpdateTrivia();
    }

    private void Update()
    {
        if (currentHoveredBall != null)
        {
            if (previewDescriptionPanel.gameObject.activeSelf == false) previewDescriptionPanel.SetActive(true);
            bool completed = Hypatios.Game.Check_TriviaCompleted(currentHoveredBall.trivia);

            if (completed)
            {
                titleLabel.text = currentHoveredBall.trivia.Title;
                descriptionLabel.text = currentHoveredBall.trivia.Description;
                image.sprite = currentHoveredBall.trivia.PreviewSprite;
                image.material = null;
            }
            else
            {
                titleLabel.text = "???";
                descriptionLabel.text = "???";
                image.sprite = cut1BeginningSprite;
                image.material = tvNoiseMat;
            }
        }
        else
        {
            if (previewDescriptionPanel.gameObject.activeSelf == true) previewDescriptionPanel.SetActive(false);
        }
    }

    public void UpdateTrivia()
    {

        var allButtons = parentTrivias.GetComponentsInChildren<TriviaBallButton>();
        allTriviaButtons = allButtons.ToList();

        GenerateMissingTrivias();
        foreach (var button in allTriviaButtons)
        {
            button.RefreshTrivia();
        }
        GenerateTriviaLine();

    }

    private void GenerateMissingTrivias()
    {
        var allMissingTrivias = Hypatios.Assets.AllTrivias.ToList();
        foreach (var button in allTriviaButtons)
        {
            allMissingTrivias.RemoveAll(x => x == button.trivia);
        }

        int i = 0;
        foreach(var _trivia in allMissingTrivias)
        {
            var pos = new Vector3(farlandPoint.transform.position.x, farlandPoint.position.y, farlandPoint.position.z);
            pos.x += (i * 1.5f);
            var newButton = Instantiate(triviaBall, parentTrivias);
            newButton.gameObject.transform.position = pos;
            newButton.trivia = _trivia;
            newButton.gameObject.name = $"TriviaBall ({_trivia.Title})";
            newButton.gameObject.SetActive(true);
            allTriviaButtons.Add(newButton);
            i++;
        }
    }

    [Button("Editor - Update trivia menu")]
    public void UpdateEditorTrivia()
    {

        var allButtons = parentTrivias.GetComponentsInChildren<TriviaBallButton>();
        allTriviaButtons = allButtons.ToList();

        GenerateTriviaLine();
    }

    private void GenerateTriviaLine()
    {
        foreach (var linerend in allLineRenderers)
        {
            if (linerend == null) continue;
            DestroyImmediate(linerend.gameObject);
        }
        allLineRenderers.RemoveAll(t => t == null);

        //check runtime
        if (Application.isPlaying == false)
        {
            var allTriviasWithRequirements = allTriviaButtons.FindAll(x => x.trivia.previousTrivia != null);

            foreach (var triviabutton1 in allTriviasWithRequirements)
            {
                var newLine = Instantiate(wire, parentLineRenders);
                var buttonPrev = FindButtonByPreviousTrivia(triviabutton1.trivia);
                var startPoint = buttonPrev.transform;
                var endPoint = triviabutton1.transform;

                newLine.origin = startPoint;
                newLine.target = endPoint;
                newLine.gameObject.SetActive(true);

                allLineRenderers.Add(newLine);
            }
        }
        else
        {
            var allValidTrivias = allTriviaButtons.FindAll(x => x.trivia.previousTrivia != null && Hypatios.Game.Check_TriviaCompleted(x.trivia));

            foreach (var triviabutton1 in allValidTrivias)
            {
                var newLine = Instantiate(wire, parentLineRenders);
                var buttonPrev = FindButtonByPreviousTrivia(triviabutton1.trivia);
                var startPoint = buttonPrev.transform;
                var endPoint = triviabutton1.transform;

                newLine.origin = startPoint;
                newLine.target = endPoint;
                newLine.gameObject.SetActive(true);

                allLineRenderers.Add(newLine);
            }
        }
    }

    private TriviaBallButton FindButtonByPreviousTrivia(Trivia currentTrivia)
    {
        return allTriviaButtons.Find(x => x.trivia == currentTrivia.previousTrivia);
    }
}
