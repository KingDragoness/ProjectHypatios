using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class WindowConfirmPrompt : MonoBehaviour
{
    [System.Serializable]
    public class ButtonCommandElement
    {
        public promptConfirmCommand delegateCommand;
        public UnityEvent OnActionCommand;
        public string[] param;
        public string buttonText;

        public ButtonCommandElement(promptConfirmCommand delegateCommand, UnityEvent onActionCommand, string _buttonText)
        {
            this.delegateCommand = delegateCommand;
            this.OnActionCommand = onActionCommand;
            this.buttonText = _buttonText;
        }
    }

    public Text label_Header;
    public Text label_Description;
    public ConfirmPromptButton buttonPrefab;
    public Transform parentButton;
    public List<ConfirmPromptButton> allButtons = new List<ConfirmPromptButton>();
    [SerializeField] [ReadOnly] private List<ButtonCommandElement> allConfirmPrompts = new List<ButtonCommandElement>();


    public delegate void promptConfirmCommand(string[] param);

    public delegate void onPromptConfirmClosed();
    public static event onPromptConfirmClosed OnConfirmPromptClosed;

    public static WindowConfirmPrompt instance
    {
        get { return Hypatios.UI.ConfirmPromptUI; }
    }

    private void Start()
    {
        buttonPrefab.gameObject.EnableGameobject(false);

    }

    public static void LaunchPrompt(string header, string description, List<ButtonCommandElement> commands)
    {
        instance.InitiatePrompt(header, description, commands);
    }

    public void InitiatePrompt(string header, string description, List<ButtonCommandElement> commands)
    {       //clear all buttons
        foreach (var button in instance.allButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allButtons.Clear();

        label_Header.text = header;

        if (string.IsNullOrEmpty(description) | string.IsNullOrWhiteSpace(description))
        {
            label_Description.gameObject.SetActive(false);
        }
        else
        {
            label_Description.gameObject.SetActive(true);
        }
        label_Description.text = description;
        allConfirmPrompts = commands;
        gameObject.SetActive(true);
        int index = 0;
        //create commands
        foreach (var command in commands)
        {
            var newButton = Instantiate(buttonPrefab, parentButton);
            newButton.transform.localScale = Vector3.one;
            newButton.label.text = command.buttonText;
            newButton.confirmUI = this;
            newButton.index = index;
            newButton.gameObject.EnableGameobject(true);
            allButtons.Add(newButton);
            index++;
        }

        RefreshUI();
    }

    public void ExecuteCommand(ConfirmPromptButton button)
    {
        var commandElement = allConfirmPrompts[button.index];

        commandElement.OnActionCommand?.Invoke();
        commandElement.delegateCommand?.Invoke(commandElement.param);
    }


    public static void CloseContextMenu()
    {
        //clear all buttons
        foreach(var button in instance.allButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        instance.allButtons.Clear();


        instance.gameObject.SetActive(false);
        OnConfirmPromptClosed?.Invoke();
    }

    public void RefreshUI()
    {

    }

    private void Update()
    {
        if (Hypatios.Input.Pause.triggered)
        {
            CloseContextMenu();
        }
    }

}
