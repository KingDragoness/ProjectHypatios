using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Speaker", order = 1)]

public class DialogSpeaker : ScriptableObject
{

    public Sprite defaultSprite;
    public List<PortraitSpeaker> allPortraits = new List<PortraitSpeaker>();

}
