using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_FreemodeControlRoom : MonoBehaviour
{
    [System.Serializable]
    public class ControlCommand
    {
        public string commandName = "Spawn Spider";
        public UnityEvent OnCommandExecute;
    }

    public List<ControlCommand> allControlCommands = new List<ControlCommand>();
    public Interact_FreemodeControl_Button buttonPrefab;
    public int limitButton = 10;
    public float buttonDistance = 0.6f;
    public Transform pivot_button;
    public Transform parent_button;

    private List<Interact_FreemodeControl_Button> _allButtons = new List<Interact_FreemodeControl_Button>();

    [ShowInInspector] private int _positionMenu = 0;
    private int selectedIndex = 0;

    private void Start()
    {
        buttonPrefab.gameObject.SetActive(false);


        Refresh_Buttons();
    }

    public void CycleButtonPos(bool isUp)
    {
        int totalPage = Mathf.CeilToInt((float)allControlCommands.Count / (float)limitButton); //FUCK YOU FOR DIVIDING IN INTEGER
        //Debug.Log(totalPage);

        if (isUp)
        {
            if (_positionMenu + 1 >= totalPage)
            {
                _positionMenu = 0;
            }
            else
            {
                _positionMenu++;
            }
        }
        else
        {
            if (_positionMenu - 1 < 0)
            {
                _positionMenu = totalPage - 1;
            }
            else
            {
                _positionMenu--;
            }
        }
        Refresh_Buttons();
    }


    public void ExecuteCommand(Interact_FreemodeControl_Button button)
    {
        ControlCommand command = allControlCommands[button.index];

        if (command == null)
        {
            Debug.Log("Command control room is broken!");
            return;
        }

        command.OnCommandExecute?.Invoke();
    }

    private void Refresh_Buttons()
    {
        foreach (var button in _allButtons)
        {
            Destroy(button.gameObject);
        }

        _allButtons.Clear();

        int curIndex = _positionMenu * limitButton;
        int upperLimit = (_positionMenu + 1) * limitButton;

        if ((_positionMenu + 1) * limitButton > allControlCommands.Count)
        {
            upperLimit = allControlCommands.Count;
        }

        for (int i = curIndex; i < upperLimit; i++)
        {
            var localIndex = i - (_positionMenu * limitButton);
            var currentCommand = allControlCommands[i];
            var newButton = Instantiate(buttonPrefab, parent_button);
            Vector3 pos = buttonPrefab.transform.position;
            pos.y -= buttonDistance * localIndex;
            {
                newButton.label_Name.text = currentCommand.commandName;
                newButton.touchable.interactDescription = $"{currentCommand.commandName}";
                newButton.transform.position = pos;
                newButton.index = i;
            }

            newButton.gameObject.SetActive(true);
            _allButtons.Add(newButton);
        }
    }


}
