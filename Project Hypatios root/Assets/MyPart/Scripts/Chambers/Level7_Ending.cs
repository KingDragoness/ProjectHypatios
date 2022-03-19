using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level7_Ending : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeShadowCustom();
    }

    
    public void ChangeShadowCustom()
    {
        QualitySettings.shadowDistance = 620;
    }

}
