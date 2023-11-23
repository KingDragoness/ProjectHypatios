using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TriviaMapUI : MonoBehaviour
{

    public TriviaBallButton currentHoveredBall;
    public TriviaMapCamera triviaCam;
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
    [FoldoutGroup("Preview")] public Vector3 offsetLook;
    public float radius_MissingTrivia = 18f;
    public float threshold_Distance = 1.5f;
    public Transform parentTrivias;
    public Transform parentLineRenders;
    [FoldoutGroup("Trivia Filters")] public Trivia.TriviaType currentFilter;
    [FoldoutGroup("Trivia Filters")] public InputField input_SearchFilter;
    [FoldoutGroup("Trivia Filters")] public Text categoryLabel;
    [FoldoutGroup("Trivia Filters")] public Transform parentTriviaButtons;
    [FoldoutGroup("Trivia Filters")] public TriviaShortButton TriviaButtonprefab;
    [FoldoutGroup("Trivia Filters")] public Color color_TriviaMain;
    [FoldoutGroup("Trivia Filters")] public Color color_TriviaSide;
    [FoldoutGroup("Trivia Filters")] public Color color_TriviaFacts;
    [FoldoutGroup("Trivia Filters")] public Sprite flagSprite;
    [FoldoutGroup("Preview Flag")] public GameObject window_PreviewFlag;
    [FoldoutGroup("Preview Flag")] public Text label_descriptionFlag;

    public List<TriviaBallButton> allTriviaButtons = new List<TriviaBallButton>();
    public List<Wire> allLineRenderers = new List<Wire>();
    [ReadOnly] public List<TriviaShortButton> allTriviaShortButtons = new List<TriviaShortButton>();
    [ReadOnly] public List<TriviaShortButton> allCodexButtons = new List<TriviaShortButton>();

    private void OnEnable()
    {
        TriviaButtonprefab.gameObject.SetActive(false);
        UpdateTrivia();
        UpdateFilterUI();
        window_PreviewFlag.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        UpdateTrivia3DMap();
    }

    #region Trivia UI

    public void ChangeCategory(int _category)
    {
        currentFilter = (Trivia.TriviaType)_category;
        UpdateFilterUI();
    }

    public void UpdateFilterUI()
    {
        categoryLabel.text = Trivia.GetTriviaName(currentFilter).ToUpper();

        foreach(var button in allTriviaShortButtons)
        {
            Destroy(button.gameObject);
        }
        foreach (var button in allCodexButtons)
        {
            Destroy(button.gameObject);
        }

        allTriviaShortButtons.Clear();
        allCodexButtons.Clear();

        foreach (var triviaClass in Hypatios.Assets.AllTrivias)
        {
            var triviaDat = Hypatios.Game.Check_TriviaCompleted(triviaClass);
            if (triviaDat == false) continue;

            if (currentFilter != Trivia.TriviaType.All)
            {
                if (currentFilter != triviaClass.TriviaCategory)
                    continue;
            }

            if (IsMatchingSearchIndex(triviaClass) == false)
                continue;

            var button1 = Instantiate(TriviaButtonprefab, parentTriviaButtons);
            button1.type = TriviaShortButton.ButtonType.Trivia;
            button1.gameObject.SetActive(true);
            button1.trivia = triviaClass;

            if (triviaClass.TriviaCategory == Trivia.TriviaType.MainStory)
                button1.icon.color = color_TriviaMain;
            else if (triviaClass.TriviaCategory == Trivia.TriviaType.SideChamber)
                button1.icon.color = color_TriviaSide;
            else if (triviaClass.TriviaCategory == Trivia.TriviaType.Facts)
                button1.icon.color = color_TriviaFacts;

            button1.Refresh();
            allTriviaShortButtons.Add(button1);
        }

        //for flags
        foreach (var flagClass in Hypatios.Assets.AllFlagSO)
        {
            var flagDat = Hypatios.Game.Check_FlagTriggered(flagClass.GetID());
            if (flagDat == false) continue;

            if (currentFilter != Trivia.TriviaType.Flags)
            {
                continue;
            }

            if (IsMatchingSearchIndex(flagClass) == false)
                continue;

            var button1 = Instantiate(TriviaButtonprefab, parentTriviaButtons);
            button1.type = TriviaShortButton.ButtonType.Flag;
            button1.gameObject.SetActive(true);
            button1.flagSO = flagClass;
            button1.icon.sprite = flagSprite;


            button1.Refresh();
            allTriviaShortButtons.Add(button1);
        }

        foreach (var codexClass in Hypatios.Assets.AllCodexHints)
        {
            var flagDat = Hypatios.Game.Check_CodexHintDone(codexClass);
            if (flagDat == false) continue;

            if (currentFilter != Trivia.TriviaType.Codex)
            {
                continue;
            }

            if (IsMatchingSearchIndex(codexClass) == false)
                continue;

            var button1 = Instantiate(TriviaButtonprefab, parentTriviaButtons);
            button1.type = TriviaShortButton.ButtonType.Codex;
            button1.gameObject.SetActive(true);
            button1.codexSO = codexClass;
            button1.icon.sprite = flagSprite;


            button1.Refresh();
            allCodexButtons.Add(button1);
        }

    }

    public bool IsMatchingSearchIndex(Trivia triviaClass)
    {


        if (string.IsNullOrEmpty(input_SearchFilter.text)) 
            return true;
        else
        {
            if (triviaClass.Title.ToLower().Contains(input_SearchFilter.text.ToLower()))
                return true;

            if (triviaClass.TriviaCategory.ToString().ToLower().Contains(input_SearchFilter.text.ToLower()))
                return true;


        }

        return false;
    }

    public bool IsMatchingSearchIndex(GlobalFlagSO flagSO)
    {


        if (string.IsNullOrEmpty(input_SearchFilter.text))
            return true;
        else
        {
            if (flagSO.DisplayName.ToLower().Contains(input_SearchFilter.text.ToLower()))
                return true;

        }

        return false;
    }

    public bool IsMatchingSearchIndex(CodexHintTipsSO codexSO)
    {


        if (string.IsNullOrEmpty(input_SearchFilter.text))
            return true;
        else
        {
            if (codexSO.Title.ToLower().Contains(input_SearchFilter.text.ToLower()))
                return true;

        }

        return false;
    }

    #endregion

    #region Highlight Flag

    public void HighlightWindow(TriviaShortButton button)
    {
        window_PreviewFlag.gameObject.SetActive(true);

        if (button.type == TriviaShortButton.ButtonType.Flag)
        {
            label_descriptionFlag.text = button.flagSO.Description;
        }
        else if (button.type == TriviaShortButton.ButtonType.Codex)
        {
            string str = "";
            str += $"<b>{button.codexSO.Title}</b> \n\n";
            str += $"{button.codexSO.Description}";
            label_descriptionFlag.text = str;
        }
    }

    public void DehighlightWindow(TriviaShortButton button)
    {
        window_PreviewFlag.gameObject.SetActive(false);

    }

    #endregion

    public void LookAtTriviaBall(TriviaShortButton triviaButton)
    {
        var ball = allTriviaButtons.Find(x => x.trivia == triviaButton.trivia);
        triviaCam.OverrideCameraTargetPos(ball.gameObject.transform.position + offsetLook);
        //triviaCam.transform.position = ball.gameObject.transform.position + offsetLook;
        triviaCam.transform.LookAt(ball.transform);
        triviaCam.ManualIntervention_Mouse();

    }

    #region Trivia 3d
    private void UpdateTrivia3DMap()
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
            Vector3 pos = new Vector3(farlandPoint.transform.position.x, farlandPoint.position.y, farlandPoint.position.z);
            int tries = 0;
            bool succeed = false;

            while (succeed == false && tries < 50f)
            {
                Vector3 _newPos = farlandPoint.position;
                _newPos.x += Random.Range(-radius_MissingTrivia, radius_MissingTrivia);
                _newPos.z += Random.Range(-radius_MissingTrivia, radius_MissingTrivia);
                
                if (IsTriviaBallColliding(_newPos) == false)
                {
                    succeed = true;
                }

                if (succeed)
                {
                    pos = _newPos;
                    break;
                }

                tries++;
            }
            
            var newButton = Instantiate(triviaBall, parentTrivias);
            newButton.gameObject.transform.position = pos;
            newButton.trivia = _trivia;
            newButton.gameObject.name = $"TriviaBall ({_trivia.Title})";
            newButton.gameObject.SetActive(true);
            allTriviaButtons.Add(newButton);
            i++;
        }
    }

    public bool IsTriviaBallColliding(Vector3 pos)
    {
        foreach (var button in allTriviaButtons)
        {
            float dist = Vector3.Distance(pos, button.transform.position);

            if (dist < threshold_Distance)
            {
                return true;
            }
        }

        return false;
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

    #endregion
}
