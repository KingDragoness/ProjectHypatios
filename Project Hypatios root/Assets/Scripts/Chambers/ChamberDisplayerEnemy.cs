using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberDisplayerEnemy : MonoBehaviour
{
    public StageChamberScript stageChamberScript;
    public ChamberText chamberText;

    private void Update()
    {
        if (stageChamberScript.enemiesToClear.Count == 0 && !stageChamberScript.Cleared)
        {

            chamberText.SetTextContent(stageChamberScript.enemiesToClear.Count.ToString());

        }
        else
        {
            chamberText.SetTextContent(stageChamberScript.enemiesToClear.Count.ToString());
        }
    }
}
