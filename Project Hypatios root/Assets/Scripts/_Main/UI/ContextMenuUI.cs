using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class ContextCommandElement
    {
        public contextMenuCommand delegateCommand;
        public string[] param;
        public string commandContextName;
        public Sprite sprite;

        public ContextCommandElement(contextMenuCommand delegateCommand, string commandContextName, Sprite _sprite = null)
        {
            this.delegateCommand = delegateCommand;
            this.commandContextName = commandContextName;
            this.sprite = _sprite;
        }
    }

    public ContextMenuButton ContextButtonTemplate;
    public Transform parentContextMenu;

    public delegate void contextMenuCommand(string[] param);

    public delegate void onContextMenuClosed();
    public static event onContextMenuClosed OnContextMenuClosed;

    private List<ContextMenuButton> allContextButtons = new List<ContextMenuButton>();

    bool dontCloseTooQuickRetard = false;

    public static ContextMenuUI instance
    { 
        get { return Hypatios.UI.ContextMenuUI; }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnContextMenuClosed = null;
    }


    public static void ShowContextMenu(Vector2 positionMouse, List<ContextCommandElement> commands)
    {
        instance.gameObject.SetActive(true);
        string s_debug = "";
        foreach(var command in commands)
        {
            s_debug += $"{command.commandContextName}";
        }
        //Generate context command element
        //instance.allContextCommands.Clear();

        instance.ClearupButton();

        foreach (var command in commands)
        {
            var button1 = Instantiate(instance.ContextButtonTemplate, instance.parentContextMenu);
            button1.labelButton.text = command.commandContextName;
            button1.contextCommand = command;
            button1.gameObject.SetActive(true);
            if (command.sprite != null)
            {
                button1.icon.gameObject.EnableGameobject(true);
                button1.icon.sprite = command.sprite;
            }
            else
            {
                button1.icon.gameObject.EnableGameobject(false);

            }
            instance.allContextButtons.Add(button1);
        }

        var rt = instance.GetComponent<RectTransform>();
        rt.anchoredPosition = positionMouse;
        instance.dontCloseTooQuickRetard = true;
        instance.antiCloseQuick = 0.25f;

    }

    public static void CloseContextMenu()
    {
        instance.gameObject.SetActive(false);
        OnContextMenuClosed?.Invoke();
    }

    private float antiCloseQuick = 0.25f;

    private void Update()
    {
        if (dontCloseTooQuickRetard)
        {
            antiCloseQuick -= Time.unscaledDeltaTime;

            if (antiCloseQuick <= 0f)
            {
                dontCloseTooQuickRetard = false;
            }

            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            var raycastES = GetEventSystemRaycastResults();
            bool close = true;

            foreach (var es in raycastES)
            {
                //Debug.Log(es.gameObject + " : " + es.gameObject.IsParentOf(this.gameObject).ToString());
                if (es.gameObject.IsParentOf(this.gameObject))
                {
                    close = false;
                    break;
                }

               
            }

            if (close)
                CloseContextMenu();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CloseContextMenu();
        }
    }

    public void ExecuteButton(ContextCommandElement commandElement)
    {
        commandElement.delegateCommand?.Invoke(commandElement.param);
    }

    private void ClearupButton()
    {
        foreach (var button in allContextButtons)
        {
            if (button == null) continue;

            Destroy(button.gameObject);

        }

        allContextButtons.Clear();
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }



}
