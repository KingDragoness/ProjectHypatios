using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elena1_SecurityCard : MonoBehaviour
{

    public Elena1_MegaofficeSecurity officeScript;

    public void ObtainCard()
    {
        officeScript.ObtainCard();
        gameObject.SetActive(false);
    }

}
