using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDialogues_ResponseSelection : MultiDialogue_Action
{

    public List<DialogCommandEntry.Branch> branches = new List<DialogCommandEntry.Branch>();

    public override DialogCommandEntry CollectEntry()
    {
        DialogCommandEntry newEntry = new DialogCommandEntry(DialogCommandEntry.Type.ResponseSelection);

        newEntry.branches = branches;

        return newEntry;
    }

}
