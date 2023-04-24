using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Queue actions like that
//Cutscene_DialogEntry = Show dialogue
//CutsceneAction_Wait = Wait command

public abstract class CutsceneAction : MonoBehaviour
{

    public CutsceneObject parentCutscene;

    public virtual void Start()
    {
        if (parentCutscene == null) parentCutscene = GetComponentInParent<CutsceneObject>();
    }


    public abstract void ExecuteAction();
    public virtual void OnDone()
    {
        //Send it to parent
    }

}
