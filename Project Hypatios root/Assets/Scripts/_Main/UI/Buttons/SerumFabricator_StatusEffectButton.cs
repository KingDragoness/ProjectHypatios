using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SerumFabricator_StatusEffectButton : MonoBehaviour
{

    public Image border;
    public Image icon;
    public Text label_PlusMinus;
    
    public void Reload(Sprite _icon, string plusString, Color _color)
    {
        border.color = _color;
        icon.color = _color;
        label_PlusMinus.color = _color;

        icon.sprite = _icon;
        label_PlusMinus.text = plusString;
    }

}
