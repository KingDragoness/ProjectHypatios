using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        CharacterScript characterScript = FindObjectOfType<CharacterScript>();

        //Restart the level for non-Aldrich levels
        if (Hypatios.Game.currentGamemode == FPSMainScript.CurrentGamemode.Aldrich)
        {
            SceneManager.LoadScene(Hypatios.Game.deadScene.Index);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
