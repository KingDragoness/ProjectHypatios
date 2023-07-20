using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ModularUI_SoulCount : MonoBehaviour
{

    public Text label_Soul;

    private void Update()
    {
        if (Mathf.Round(Time.time * 10) % 2 == 0) return;

        label_Soul.text = $"{Hypatios.Game.SoulPoint}";
    }

}
