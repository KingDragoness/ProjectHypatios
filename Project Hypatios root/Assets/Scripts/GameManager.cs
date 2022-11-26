using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int deadLevelIndex = 1;

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        CharacterScript characterScript = FindObjectOfType<CharacterScript>();

        //Restart the level for non-Aldrich levels
        if (FPSMainScript.instance.currentGamemode == FPSMainScript.CurrentGamemode.Aldrich)
        {
            SceneManager.LoadScene(deadLevelIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
