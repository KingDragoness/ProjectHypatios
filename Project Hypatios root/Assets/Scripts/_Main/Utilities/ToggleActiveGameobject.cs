using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveGameobject : MonoBehaviour
{
    
    public void ToggleGameobjectActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

}
