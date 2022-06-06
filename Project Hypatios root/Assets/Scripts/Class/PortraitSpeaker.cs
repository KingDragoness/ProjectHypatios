using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Speaker Portrait", order = 1)]
public class PortraitSpeaker : ScriptableObject
{
    [PreviewField(100)]
    public Sprite portraitSprite;
}
