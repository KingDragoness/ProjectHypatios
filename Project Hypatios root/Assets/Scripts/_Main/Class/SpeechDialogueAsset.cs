using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "VaultUnlockFailed", menuName = "Hypatios/Speech Dialogue", order = 1)]
public class SpeechDialogueAsset : ScriptableObject
{
   
    [System.Serializable]
    public class EntryDialogue
    {
        [HideLabel] public DialogSpeaker dialogSpeaker;
        [TextArea(3, 4)]
        public string Dialogue_Content;
        [FoldoutGroup("Show More")] public PortraitSpeaker portraitSpeaker;
        [FoldoutGroup("Show More")] public AudioClip dialogAudioClip;
        [FoldoutGroup("Show More")] public float Dialogue_Timer = 4;


    }


    [ListDrawerSettings(DraggableItems = true, Expanded = false, ShowPaging = false, ShowItemCount = false)] public List<EntryDialogue> entryDialogues = new List<EntryDialogue>();
    public bool cannotWhenTalkingToOther = false;
    public bool isImportant = true;


    [Button("Trigger Message")]
    public void TriggerMessage()
    {

        if (Hypatios.Dialogue.IsTalking() && cannotWhenTalkingToOther)
        {
            return;
        }

        foreach (var dialog in entryDialogues)
        {
            Sprite portrait = null;

            if (dialog.portraitSpeaker == null)
            {
                portrait = dialog.dialogSpeaker.defaultSprite;
            }
            else
            {
                portrait = dialog.portraitSpeaker.portraitSprite;
            }

            Hypatios.Dialogue.QueueDialogue(dialog.Dialogue_Content,
                dialog.dialogSpeaker.name,
                dialog.Dialogue_Timer,
                portrait,
                dialog.dialogAudioClip, priorityLevel: 100,
                isImportant: isImportant,
                false);

        }

    }
}
