using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using Sirenix.OdinInspector;

public class MachineMadnessWeapon : GunScript
{


    public Animator machine_Animator;
    public float HoldTimeToInitiate = 1f;
    public float TimeToTransitScene = 5f;
    public AnalogueClockTimeHand clockHand;
    public AnimatorSetBool anim_RotatorKeyInitiate;
    public AutoMoveAndRotate autoHand1;
    public AutoMoveAndRotate autoHand2;
    public AutoMoveAndRotate autoHand3;

    public static MachineMadnessWeapon Instance;

    private bool _INITIATEFUCKING_vortex = false;
    private bool _initiateFUCKING_SceneTransition = false;
    private HypatiosSave cachedHypatiosSave;
    private float _timeHolding = 0f;

    private void Start()
    {
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _timeHolding = 0f;
        anim_RotatorKeyInitiate.SetBool(false);
    }

    public override void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        if (_INITIATEFUCKING_vortex == true)
        {
            machine_Animator.SetBool("TimeVortex", true);
            autoHand1.enabled = true;
            autoHand2.enabled = true;
            autoHand3.enabled = true;

            if (_initiateFUCKING_SceneTransition == false)
            {
                StartCoroutine(InitiateSceneTransition());
                _initiateFUCKING_SceneTransition = true;
            }

            return;
        }


        if (Hypatios.Player.disableInput)
            return;

        if (Hypatios.Player.Weapon.disableInput)
            return;


        clockHand.DEBUG_ClockHand();

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Holster"))
        {
            //Hold for 1 seconds
            if (Hypatios.Input.Fire1.IsPressed())
            {
                _timeHolding += Time.deltaTime;
                anim_RotatorKeyInitiate.SetBool(true);
                if (audioFire.isPlaying == false) audioFire?.Play();

                if (_timeHolding > HoldTimeToInitiate)
                {
                    InitiateMachineUI();
                }
            }
            else
            {
                _timeHolding = 0f;
                if (audioFire.isPlaying) audioFire.Stop();
                anim_RotatorKeyInitiate.SetBool(false);
            }
        }

    }

    private void InitiateMachineUI()
    {
        Debug.Log("Initiate Machine of Madness");
        Hypatios.UI.ChangeCurrentMode(MainUI.UIMode.MachineOfMadness);
    }

    public void InitiateTimeVortex(HypatiosSave hypatiosSave)
    {
        _INITIATEFUCKING_vortex = true;
        cachedHypatiosSave = hypatiosSave;
    }

    public IEnumerator InitiateSceneTransition()
    {
        yield return new WaitForSeconds(TimeToTransitScene);
        //establish new save
        Hypatios.Game.MachineMadnessSave(cachedHypatiosSave);
        yield return new WaitForSeconds(0.1f);
        Application.LoadLevel(cachedHypatiosSave.Game_LastLevelPlayed);
    }
}
