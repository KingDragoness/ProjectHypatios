using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FW_UI_FollowingGuardstar : MonoBehaviour
{

    public List<GameObject> guardstarIcons = new List<GameObject>();
    public UI_Modular_ShowTempCanvas modularUI_Show;

    private void Start()
    {
        Chamber_Level7.instance.onModifiedFollower += GuardstarCountModified;
    }

    private void Update()
    {
        foreach (var icon in guardstarIcons)
        { 
            icon.gameObject.SetActive(false);
        }

        int x = 0;

        foreach(var icon in guardstarIcons)
        {
            if (x >= Chamber_Level7.instance.FollowerCount)
            {
                break;
            }

            guardstarIcons[x].gameObject.SetActive(true);
            x++;
        }


    }

    public void GuardstarCountModified()
    {
        modularUI_Show.Show();
    }


}
