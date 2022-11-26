using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParadoxShopOwner : MonoBehaviour
{

    public GameObject vc_ParadoxShow;
    public Transform tpPlayerHere;
    public List<ParadoxLevelScript> paradoxLevelScripts = new List<ParadoxLevelScript>();
    public OnTriggerEnterEvent triggerEvent;
    public ParadoxLevelScript selectedParadox;
    public bool DEBUG_AlwaysOn = false;

    public static ParadoxShopOwner Instance;

    private bool b1 = false;
    private bool b2 = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!DEBUG_AlwaysOn && FPSMainScript.instance.TotalRuns < 1)
        {
            gameObject.SetActive(false);
        }
        
        if (Application.isEditor)
        {
            gameObject.SetActive(true);
        }

        RefreshAllParadoxes();

        if (triggerEvent != null)
        {
            triggerEvent.objectToCompare = FindObjectOfType<CharacterScript>().gameObject;
        }
    }

    public void OpenParadox()
    {
        MainUI.Instance.ChangeCurrentMode(2);

    }

    public void RefreshAllParadoxes()
    {
        paradoxLevelScripts = FindObjectsOfType<ParadoxLevelScript>().ToList();
    }    

    public void EnableStateParadox()
    {
        vc_ParadoxShow.gameObject.SetActive(true);

        if (b2)
        {

            b2 = false;
        }

        b1 = true;
    }
    public void DisableStateParadox()
    {
        vc_ParadoxShow.gameObject.SetActive(false);

        if (b1)
        {
            var player = FindObjectOfType<CharacterScript>();
            player.transform.position = tpPlayerHere.transform.position;
            b1 = false;
        }

        b2 = true;
    }
}
