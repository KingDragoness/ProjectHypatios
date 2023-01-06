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
