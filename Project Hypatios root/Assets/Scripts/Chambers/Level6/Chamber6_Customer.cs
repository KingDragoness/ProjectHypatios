using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Sirenix.OdinInspector;

public class Chamber6_Customer : EnemyScript
{

    public enum AIMode
    {
        GetFood,
        Eating,
        Escape,
        ReturnBay
    }

    [FoldoutGroup("DEBUG")] public Chamber6_Piring DEBUG_PiringToCheck;
    [FoldoutGroup("AI")] public AIMode mode;
    [FoldoutGroup("AI")] public Transform targetSeat;
    [FoldoutGroup("AI")] public Transform exitCargo;
    [FoldoutGroup("AI")] public Transform target;
    [FoldoutGroup("AI")] public RandomSpawnArea randomEscapeRoute;
    [FoldoutGroup("AI")] public NavMeshAgent agent;
    [FoldoutGroup("Setup")] public Transform piringPlace;
    [FoldoutGroup("Setup")] public Transform customerWreckage;
    [FoldoutGroup("Setup")] public Animator anim;
    [FoldoutGroup("Setup")] public AnimatorOverrideController[] animOverides;
    [FoldoutGroup("Setup")] public Chamber_Level6.Order order;
    [FoldoutGroup("Setup")] public Chamber_Level6 chamberScript;
    public int tableSeat = 0;

    private float cooldown = 0.08f;
    private Chamber6_Piring myPiring;
    private int random1;
    private float cooldownEscape = 10f;
    private bool alreadyEat = false;
    private bool isSitting = false;
    private bool orderAlreadyTaken = false;
    private float distance = 1;
    public bool AlreadyEat { get => alreadyEat;}

    private void Start()
    {
        cooldown = 0.08f;
        random1 = Random.Range(-1, animOverides.Length);

        if (random1 != -1)
        {
            anim.runtimeAnimatorController = animOverides[random1];
        }
    }

    private bool escapeMode = false;
    [ReadOnly] public Vector3 targetEscapeRoute;
    private float eatingTimer = 6f;

    public void Update()
    {
        if (isAIEnabled == false) return;

        distance = Vector3.Distance(transform.position, target.position);
        cooldown -= Time.deltaTime;

        if (mode != AIMode.Escape)
        {
            agent.SetDestination(target.position);

            if (distance > 1.5f)
                agent.autoBraking = false;
            else
                agent.autoBraking = true;
        }

        if (cooldown < 0f)
        {
            if (mode == AIMode.GetFood)
                DetectNearbyPlates();

            if (mode == AIMode.ReturnBay)
                Run_ReturnBay();

            cooldown = 0.08f;
        }

        if (mode == AIMode.Escape)
        {
            cooldownEscape -= Time.deltaTime;
            Escaping();

            if (cooldownEscape < 0f)
            {
                if (!AlreadyEat)
                    mode = AIMode.GetFood;
                else
                    mode = AIMode.ReturnBay;
            }
        }
        else
        {
            escapeMode = false;
        }

        if (mode == AIMode.Eating)
            Eating();

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        float distToSeat = Vector3.Distance(transform.position, targetSeat.position);
        bool isSeat = false;

        if (agent.velocity.magnitude < 5 && distToSeat < 2f)
        {
            isSeat = true;
        }

        if (isSeat)
        {
            anim.SetBool("Sit", true);
            isSitting = true;
        }
        else
        {
            anim.SetBool("Sit", false);
            isSitting = false;
        }

        if (mode == AIMode.Eating)
        {
            anim.SetBool("Eat", true);
        }
        else
        {
            anim.SetBool("Eat", false);
        }
    }

    #region Enemy Base

    public override void Attacked(DamageToken token)
    {
        Stats.CurrentHitpoint -= token.damage;
        if (token.origin == DamageToken.DamageOrigin.Player) DamageOutputterUI.instance.DisplayText(token.damage);
        cooldownEscape = 10f;
        mode = AIMode.Escape;

        if (Stats.CurrentHitpoint < 0)
        {
            KillCustomer();
        }

        if (orderAlreadyTaken)
        {
            mode = AIMode.ReturnBay;
        }

        base.Attacked(token);
    }


    #endregion

    float cooldownOptimizeEscape = 0.1f;


    private void Escaping()
    {
        cooldownOptimizeEscape -= Time.deltaTime;

        bool sample = false;
        if (escapeMode == false)
        {
            targetEscapeRoute = randomEscapeRoute.GetAnyPositionInsideBox();
            sample = true;
            escapeMode = true;
        }

        agent.SetDestination(targetEscapeRoute);

        if (cooldownOptimizeEscape > 0) return;

        float dist = Vector3.Distance(transform.position, targetEscapeRoute);

        if (dist < 3) sample = true;

        if (!CanPathBeReached(targetEscapeRoute)) sample = true;

        if (sample)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 25f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
            Vector3 finalPosition = hit.position;

            targetEscapeRoute = finalPosition;
        }

        cooldownOptimizeEscape = 0.1f;
    }

    bool RandomPoint(Vector3 v3, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(v3, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    bool CanPathBeReached(Vector3 target)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        //create path and check if it can be done
        // and check if navMeshAgent can reach its target
        if (agent.CalculatePath(target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            //move to target
            return true;
        }

        return false;
    }


    private void Eating()
    {
        if (eatingTimer > 0)
        {
            eatingTimer -= Time.deltaTime;
            transform.rotation = target.transform.rotation;
        }
        else
        {
            Destroy(piringPlace.gameObject);
            alreadyEat = true;
            mode = AIMode.ReturnBay;
            UpdateAI();
        }
    }

    private void Run_ReturnBay()
    {
        float dist = Vector3.Distance(transform.position, exitCargo.transform.position);

        if (dist < 1)
        {
            LeaveBay();
        }
    }

    private void DetectNearbyPlates()
    {
        for(int x = chamberScript.piringList.Count() - 1; x >= 0; x--)
        {
            var piring = chamberScript.piringList[x];

            if (piring == null)
            {
                chamberScript.RefreshList();
                continue;
            }

            float dist = Vector3.Distance(transform.position, piring.transform.position);

            if (dist < 3 && isSitting)
            {
                var isMyOrder = CheckPlateValid(piring);
                if (isMyOrder)
                {
                    myPiring = piring;
                    TakePlate(); }
            }
        }
    }

    private void TakePlate()
    {
        if (chamberScript.mainPiring != myPiring)
        {
        }
        else
        {
            var piring1 = Instantiate(chamberScript.servingPiring);
            piring1.CopyIngredient(myPiring);
            piring1.gameObject.SetActive(true);
            chamberScript.PutOffPiring();

            myPiring = piring1;
        }

        myPiring.waypointScript.enabled = false;
        myPiring.transform.SetParent(piringPlace.transform);
        myPiring.transform.position = piringPlace.transform.position;
        myPiring.ChangeOwnership(this.transform);
        orderAlreadyTaken = true;
        mode = AIMode.Eating;
        DialogueSubtitleUI.instance.QueueDialogue($"Beep! Order #{(tableSeat + 1).ToString("00")} has been taken.", "Zart Bot", 3f);
        UpdateAI();
    }

    private void KillCustomer()
    {
        if (Stats.IsDead )
            return;
        Die();

    }

    public override void Die()
    {
        OnDied?.Invoke();
        Stats.IsDead = true;
        DialogueSubtitleUI.instance.QueueDialogue($"Error! A customer has been killed!", "Zart Bot", 3f);
        MainGameHUDScript.Instance.audio_Error.Play();
        chamberScript.allCustomers.Remove(this);
        var wreckage = Instantiate(customerWreckage.gameObject);
        wreckage.transform.position = transform.position;
        wreckage.transform.rotation = transform.rotation;
        wreckage.gameObject.SetActive(true);
        chamberScript.OnCustomerLeaving?.Invoke();
        chamberScript.remainingCustomers++;
        chamberScript.RefreshList();

        Destroy(gameObject);
    }

    private void LeaveBay()
    {
        chamberScript.allCustomers.Remove(this);
        Destroy(gameObject);
        chamberScript.OnCustomerLeaving?.Invoke();
        chamberScript.RefreshList();
    }


    [FoldoutGroup("DEBUG")] [Button("Debug_CheckPlate")]
    public void DEBUG_CheckPlate()
    {
        var leftover = DEBUG_PiringToCheck.ingredients.Except(order.allRecipes).ToList();

        if (leftover.Count == 0)
        {
            //matched
        }
    }

    public void UpdateAI()
    {
        if (mode == AIMode.GetFood)
        {
            target = targetSeat;
        }
        else if (mode == AIMode.ReturnBay)
        {
            target = exitCargo;
        }
    }

    public bool CheckPlateValid(Chamber6_Piring piring)
    {
        bool valid = false;

        var leftover = order.allRecipes.Except(piring.ingredients).ToList();
        var leftover1 = piring.ingredients.Except(order.allRecipes).ToList();

        if (leftover.Count == 0 && leftover1.Count == 0)
            valid = true;

        return valid;
    }
}
