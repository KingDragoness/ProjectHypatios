using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSGame
{

    public class MenuUITest : MonoBehaviour
    {

        public GameObject MenuUI;
        public GameObject HUDUI;
        public GameObject Player;
        private bool paused = false;

        public void GamePause(bool doPause = false)
        {
            paused = !paused;
            RefreshPauseState();

        }


        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                paused = !paused;

                RefreshPauseState();
            }

        }

        private void RefreshPauseState()
        {
            if (paused == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                MenuUI.gameObject.SetActive(false);
                Time.timeScale = 1;
                //Player.gameObject.SetActive(true);
                HUDUI.gameObject.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                MenuUI.gameObject.SetActive(true);
                Time.timeScale = 0;

                //Player.gameObject.SetActive(false);
                HUDUI.gameObject.SetActive(false);


            }
        }

        public void Demo_PlayLevel1()
        {
            Application.LoadLevel(1);
        }

    }

}