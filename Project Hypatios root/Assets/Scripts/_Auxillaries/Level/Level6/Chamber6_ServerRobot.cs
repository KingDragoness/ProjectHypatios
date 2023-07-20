using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Chamber6_ServerRobot : MonoBehaviour
{



    public Chamber_Level6 chamberScript;
    public Transform piringPlace;
    public TextMesh monitorText;
    public Transform standbyPoint;

    private Chamber6_Piring targetPiring;
    private Chamber6_Customer targetCustomer;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        chamberScript.allServos.Add(this);
    }

    private void Update()
    {
        RunAI();
    }


    private void RunAI()
    {
        if (targetCustomer != null)
        {
            agent.SetDestination(targetCustomer.transform.position);

            if (targetCustomer.AlreadyEat)
            {
                targetCustomer = null;
                TrashThePlate();
            }
        }
        else
        {
            if (targetPiring != null) TrashThePlate();

            agent.SetDestination(standbyPoint.transform.position);
            monitorText.text = $"Ready.";
        }
    }

    public void DeliverPlate()
    {

        if (IsAnyOrderMatch(chamberScript.mainPiring) == false)
        {
            Hypatios.Dialogue.QueueDialogue($"Error! No customer match your plate!", "Zart Bot", 3f);
            targetPiring = null;
            return;
        }

        TakePlate();
        targetCustomer = GetCustomerThatMatch(chamberScript.mainPiring);
        targetPiring.OnTransferPlate += TargetPiring_OnTransferPlate;

        Hypatios.Dialogue.QueueDialogue($"Beep! Delivering for Order #{(targetCustomer.tableSeat + 1).ToString("00")}.", "Zart Bot", 3f);
        monitorText.text = $"Order #{(targetCustomer.tableSeat + 1).ToString("00")}";
    }

    private void TargetPiring_OnTransferPlate(Transform owner)
    {
        Chamber6_Customer customer = owner.GetComponent<Chamber6_Customer>();

        if (customer != null)
        {
            //delivered!
            if (customer == targetCustomer)
            {
                targetCustomer = null;
                targetPiring = null;
                //TrashThePlate();
            }
        }
    }

    private void TrashThePlate()
    {
        if (targetPiring != null) Destroy(targetPiring.gameObject);

    }

    private void TakePlate()
    {
        TrashThePlate();

        {
            var piring1 = Instantiate(chamberScript.servingPiring);
            piring1.CopyIngredient(chamberScript.mainPiring);
            piring1.gameObject.SetActive(true);
            chamberScript.PutOffPiring();
            targetPiring = piring1;
        }

        targetPiring.waypointScript.enabled = false;
        targetPiring.transform.SetParent(piringPlace.transform);
        targetPiring.transform.position = piringPlace.transform.position;
        targetPiring.ChangeOwnership(this.transform);
        chamberScript.piringList.Add(targetPiring);
        chamberScript.RefreshList();
        chamberScript.OnCustomerChanged?.Invoke();
    }

    private bool IsAnyOrderMatch(Chamber6_Piring piring)
    {

        foreach(var customer in chamberScript.allCustomers)
        {
            if (customer.AlreadyEat) continue;

            if (customer.CheckPlateValid(piring))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOrderMatch(Chamber6_Customer customer)
    {
        if (targetPiring == null) return false;
        if (customer.CheckRecipesValid(targetPiring.ingredients))
        {
            return true;
        }
        return false;
    }


    private Chamber6_Customer GetCustomerThatMatch(Chamber6_Piring piring)
    {
        foreach (var customer in chamberScript.allCustomers)
        {
            if (customer.AlreadyEat) continue;

            if (customer.CheckPlateValid(targetPiring))
            {
                return customer;
            }
        }

        return null;
    }

}
