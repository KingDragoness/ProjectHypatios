using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerKilledBySpider", menuName = "Hypatios/Timeline Event", order = 1)]
public class TimelineEvent : ScriptableObject
{
    public enum TriviaType
    {
        GameStory,
        GameMechanic
    }

    [SerializeField] private string _id = "Chamber1.Completed";
    public bool disableEvent = false;

    public string ID { get => _id; }
}
