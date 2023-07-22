using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Trivia_Chamber1Completed", menuName = "Hypatios/Trivia", order = 1)]
public class Trivia : ScriptableObject
{

    public enum TriviaType
    {
        MainStory = 0,
        SideChamber = 20,
        Facts = 100
    }

    public static string GetTriviaName(TriviaType type)
    {
        if (type == TriviaType.MainStory)
        {
            return "Main Story";
        }
        else if (type == TriviaType.SideChamber)
        {
            return "Side Story";
        }
        else if (type == TriviaType.Facts)
        {
            return "Facts";
        }

        return "NULL";
    }

    [SerializeField] private string _id = "Chamber1.Completed";
    [SerializeField] private string _title = "Chamber 1";
    [SerializeField] private Sprite _spriteIcon;
    [SerializeField] private Sprite _spritePreviewImage;
    [TextArea(2,4)] [SerializeField] private string _description = "I've completed the first chamber of the game.";
    [SerializeField] private TriviaType _triviaType;
    public bool disableTrivia = false;
    public Trivia previousTrivia;

    public string ID { get => _id;  }
    public string Title { get => _title;  }
    public Sprite SpriteIcon { get => _spriteIcon; }
    public Sprite PreviewSprite { get => _spritePreviewImage; }
    public string Description { get => _description;  }
    public TriviaType TriviaCategory { get => _triviaType; }

    [FoldoutGroup("Debug")]
    [Button("Trigger Trivia")]
    public void TriggerTrivia()
    {
        Hypatios.Game.TriviaComplete(this);
    }


}
