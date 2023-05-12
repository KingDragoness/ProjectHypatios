using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_SavePoint : MonoBehaviour
{

    public AnimatorSetBool animatorSetBool;

    private bool isHover = false;

    public void Hover()
    {
        isHover = true;

        Hypatios.Game.RuntimeTutorialHelp("Save Point", "You can save your progress in the middle of a level. Your inventory, weapons, trivias will be saved however " +
            " keep in mind your progress in the level will not always be saved when loaded.", "SaveCheckPoint");

    }

    public void NotHover()
    {
        isHover = false;
    }

    private void Update()
    {
        if (!isHover)
            animatorSetBool.SetBool(false);
        else
            animatorSetBool.SetBool(true);


    }

    public void SaveGame()
    {
        HypatiosSave.EntryCache token = new HypatiosSave.EntryCache();
        token.entryIndex = -1;
        token.cachedPlayerPos = Hypatios.Player.transform.position;
        Hypatios.Game.SaveGame(EntryToken: token);
    }
}
