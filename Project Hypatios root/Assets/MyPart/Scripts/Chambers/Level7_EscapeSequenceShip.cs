using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level7_EscapeSequenceShip : MonoBehaviour
{

    public characterScript playerScript;

    void Start()
    {
        playerScript.transform.SetParent(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
