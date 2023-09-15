using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Sirenix.OdinInspector;

public class Level_CollateralDreams : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    private float _time = 0f;

    private void Update()
    {

        _time += Time.deltaTime;
    
        if (_time > 2f && videoPlayer.isPlaying == false)
        {
            //Quits the game and delibrately crashes.
            Application.Quit();
            Debug.Log("Quit Game");
        }
    }



}
