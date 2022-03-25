using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class MainMenuTitleScript : MonoBehaviour
{

    public GameObject resumeButton;
    public GameObject fileExistPrompt;

    private bool savefileExist = false;

    private void Start()
    {
        string pathLoad = "";
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        pathLoad = FPSMainScript.GameSavePath + "/defaultSave.save";

        try
        {
            var tempSave = JsonConvert.DeserializeObject<HypatiosSave>(File.ReadAllText(pathLoad), settings);

            if (tempSave == null)
            {
                savefileExist = false;

            }
            else
            {
                savefileExist = true;
            }

        }
        catch
        {
            savefileExist = false;

        }

        if (savefileExist == false)
        {
            resumeButton.gameObject.SetActive(false);
        }
    }

    public void CheckAndNewGame()
    {
        if (savefileExist)
        {
            fileExistPrompt.SetActive(true);

        }
        else
        {
            FPSMainScript.instance.Menu_StartPlayGame();
        }
    }

}
