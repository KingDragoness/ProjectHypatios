using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "WING Corps", menuName = "Hypatios/StockCompany", order = 1)]
public class StockExchange_ProfileObject : ScriptableObject
{

    [InfoBox("Index always four letters.")]
    public string indexID = "WING"; 
    public string companyDisplayName = "WING Corps";
    public Sprite companyLogo;
    [TextArea(2,3)] public string companyDescription = "Manufactures infantry weapon, heavy gun, artillery and ammunition.";
    public int seed = 128101;

    [Space]
    [InfoBox("1 souls = 5 centurion")]
    public float startingSharePrice = 93.5f;
    public float marketCap = 322.4f; //in billions, Centurion
    public bool isMobiusCorps = false; //Mobius Corps falling 1% per 5 minutes

}
