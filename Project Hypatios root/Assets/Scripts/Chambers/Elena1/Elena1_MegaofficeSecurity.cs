using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elena1_MegaofficeSecurity : MonoBehaviour
{
    public bool HasObtainedSecurity = false;
    public List<Transform> spawnPositionSecurityCards = new List<Transform>();
    public GameObject securityCard;

    private void Start()
    {
        Transform t = spawnPositionSecurityCards[Random.Range(0, spawnPositionSecurityCards.Count)];

        securityCard.transform.position = t.position;
    }


    public void ObtainCard()
    {
        HasObtainedSecurity = true;
    }
}
