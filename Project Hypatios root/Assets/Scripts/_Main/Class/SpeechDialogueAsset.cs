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
        [TextArea(3, 4)]
        public string Dialogue_Content;
        public DialogSpeaker dialogSpeaker;
        public PortraitSpeaker portraitSpeaker;
        public AudioClip dialogAudioClip;
        public float Dialogue_Timer = 4;
    }

    public List<EntryDialogue> entryDialogues = new List<EntryDialogue>();
    public bool shouldOverride = false;
    public bool isImportant = true;

    [Button("Trigger Message")]
    public void TriggerMessage()
    {

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

            DialogueSubtitleUI.instance.QueueDialogue(dialog.Dialogue_Content,
                dialog.dialogSpeaker.name,
                dialog.Dialogue_Timer,
                portrait,
                dialog.dialogAudioClip, priorityLevel: 100,
                isImportant: isImportant,
                shouldOverride: shouldOverride);

        }

    }
}
