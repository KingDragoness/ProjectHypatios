using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class StatEntryButton : MonoBehaviour
{

    public Text label_Name;
    public Text label_Value;
    public bool isPersistent = false;
    public string ID = "";

    public void Refresh()
    {
        var statEntry = Hypatios.Assets.GetStatEntry(ID);
        var statData = Hypatios.Game.Get_StatEntryData(statEntry, isPersistent);

        label_Name.text = statEntry.displayText;

        if (statData == null)
        {
            label_Value.text = "0";
            return;
        }

        label_Value.text = $"{statData.value_int}";
    }

}
