using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber6_Ingredient : InteractableObject
{

    public Chamber_Level6 ChamberScript;
    public Chamber_Level6.Ingredient ingredient;

    [FoldoutGroup("Setup")] public TextMesh textMesh;
    [FoldoutGroup("Setup")] public MeshRenderer meshrenderer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override string GetDescription()
    {
        return ingredient.ToString();
    }

    public override void Interact()
    {
        if (ChamberScript.currentMode != Chamber_Level6.PiringMode.PiringTaken)
        { DialogueSubtitleUI.instance.QueueDialogue("You need to have a plate first.", "SYSTEM", 3f); return; }

        if (ingredient != Chamber_Level6.Ingredient.Rice)
        {
            if (ChamberScript.mainPiring.HasIngredient(Chamber_Level6.Ingredient.Rice) == false)
            {
                DialogueSubtitleUI.instance.QueueDialogue("You need to add rice first.", "SYSTEM", 3f);

                return;
            }
        }

        //take ingredient
        ChamberScript.mainPiring.AddIngredient(ingredient);
    }
}
