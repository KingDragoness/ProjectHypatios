using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ContextMenuButton : MonoBehaviour
{

    public Text labelButton;
    public ContextMenuUI contextMenuUI;

    private List<string> paramList = new List<string>();

    public List<string> ParamList { get => paramList; }

    public ContextMenuUI.ContextCommandElement contextCommand;


    public void ExecuteButton()
    {
        contextMenuUI.ExecuteButton(contextCommand);
        ContextMenuUI.CloseContextMenu();
    }

}
