using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv2MinigameCasino_Shootable : Enemy
{

    public enum Type
    {
        Terrorist,
        Civilian
    }

    public float HP = 100;
    public float speed = 4;
    public float delay = 1;
    public Type type = Type.Terrorist;
    public Transform targetMove;
    public Level2MinigameCasino casinoScript;
    public iTween.EaseType easeType = iTween.EaseType.linear;
    public iTween.LoopType loopType = iTween.LoopType.pingPong;
    public float timerDropEnemy = 0;

    private float timer = 0;
    private bool allowShoot = false;

    private void Start()
    {
        MoveShootable();
    }

    private bool dropped = false;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1)
        {
            allowShoot = true;
        }

        if (timer > timerDropEnemy)
        {
            if (dropped == false)
            {
                Drop();
            }

            if (timer > timerDropEnemy + 3)
            {
                Destroy(gameObject);
            }
        }
    }

    [ContextMenu("MoveShootable")]
    public void MoveShootable()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", targetMove, "speed", speed, "easetype", easeType, "looptype", loopType, "delay", delay));

    }

    public void SinkToGround()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + new Vector3(0, -5, 0), "speed", speed * 2, "easetype", iTween.EaseType.linear));

    }

    public override void Attacked(float damage, float repulsionForce = 1)
    {
        HP -= damage;
        DamageOutputterUI.instance.DisplayText(damage);
        ProcessDamage();
    }

    private void ProcessDamage()
    {
        if (HP <= 0)
        {
            Drop();
            if (type == Type.Terrorist)
            {
                casinoScript.RewardPlay(2);
                MainGameHUDScript.Instance.audio_PurchaseReward.Play();
            }
            else if (type == Type.Civilian)
            {
                casinoScript.PenalizedPlay(4);
                MainGameHUDScript.Instance.audio_Error.Play();
                DialogueSubtitleUI.instance.QueueDialogue($"Don't shoot civilians!", "SYSTEM", 5f);
            }
            //die
        }
    }

    private void Drop()
    {
        allowShoot = false;
        SinkToGround();
        dropped = true;
        Destroy(gameObject, 3f);

    }



}
