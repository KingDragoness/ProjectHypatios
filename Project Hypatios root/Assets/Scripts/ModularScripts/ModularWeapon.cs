using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSGame
{

    [System.Serializable]
    public class AnimationPlay
    {
        public string actionName = "idle";
        public string animationName = "Armature|Idle";
    }

    public class ModularWeapon : MonoBehaviour
    {

        public List<AnimationPlay> AnimationPlays = new List<AnimationPlay>();
        public bool isSinglePlay = false;
        public Animation weaponAnimation;
        public GameObject weaponOutMuzzle;

        private void Update()
        {
            //if (isSinglePlay)
            //{
            //    if (Input.GetButtonUp("Fire1"))
            //    {
            //        Fire();
            //    }
            //}
            //else
            //{
            //    if (Input.GetButton("Fire1"))
            //    {
            //        if (!weaponAnimation.isPlaying)
            //            Fire();
            //    }
            //}
        }

        public void Play()
        {
            Fire();
        }

        private void Fire()
        {
            var plays = AnimationPlays.FindAll(x => x.actionName == "Fire");
            var play = plays[Random.Range(0, plays.Count)];
            weaponAnimation.Play(play.animationName);
            weaponOutMuzzle.gameObject.SetActive(true);
        }

        public bool IsAnimationPlaying()
        {
            return weaponAnimation.isPlaying;
        }
    }

}