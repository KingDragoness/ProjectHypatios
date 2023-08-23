using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_NarakaBar_Button : MonoBehaviour
{

    public Interact_NarakaBar narakaBarScript;
    public int index = 0;
    public MeshRenderer iconMesh;
    public TextMesh label_Soul;
    public TextMesh label_ItemName;
    public Interact_Touchable touchable;

    public void BuyDrink()
    {
        narakaBarScript.BuyDrink(this);
    }

}
