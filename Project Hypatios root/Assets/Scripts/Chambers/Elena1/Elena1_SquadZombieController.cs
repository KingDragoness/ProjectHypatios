using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Elena1_SquadZombieController : MonoBehaviour
{

    public enum SquadMode
    {
        SplitUp,
        DirectAttack
    }

    public List<Monster_ZombieMobius> AllZombies = new List<Monster_ZombieMobius>();
    public Transform playerTarget;
    public float splitUpRange = 15f;
    public List<Transform> guardPositions = new List<Transform>();
    public SquadMode squadMode = SquadMode.DirectAttack;

    private float cooldownSquad = 5f;
    private float timerCooldownSquad = 4f;
    private float timerCooldownCheck = .5f;


    private void Start()
    {
        
    }

    private void Update()
    {
        timerCooldownSquad += Time.deltaTime;
        timerCooldownCheck += Time.deltaTime; 
        
        if (timerCooldownSquad > cooldownSquad)
        {
            AllZombies.RemoveAll(x => x == null);

            DecideSquadMode();
            timerCooldownSquad = 0;
        }

        if (timerCooldownCheck > .5f)
        {

            AllZombies.RemoveAll(x => x == null);

            foreach (var zombie in AllZombies)
            {
                if (Vector3.Distance(zombie.transform.position, playerTarget.position) < splitUpRange)
                {
                    zombie.SetTarget(null);
                }
            }
            timerCooldownCheck = 0;
        }

    }

    private void DecideSquadMode()
    {
        float random1 = Random.Range(0f, 1f);

        if (random1 < 0.5f)
        {
            squadMode = SquadMode.DirectAttack;
        }
        else
        {
            squadMode = SquadMode.SplitUp;

        }

        if (squadMode == SquadMode.DirectAttack)
        {
            foreach (var zombie in AllZombies)
            {
                zombie.SetTarget(null);
            }
        }
        else if (squadMode == SquadMode.SplitUp)
        {
            foreach (var zombie in AllZombies)
            {
                float random2 = Random.Range(0f, 1f);

                if (random2 < 0.8f && Vector3.Distance(zombie.transform.position, playerTarget.position) < splitUpRange)
                {
                    zombie.SetTarget(null);
                }
                else
                {
                    Transform targetGuard = guardPositions[Random.Range(0, guardPositions.Count)];

                    zombie.SetTarget(targetGuard);
                }
            }
        }

    }


}
