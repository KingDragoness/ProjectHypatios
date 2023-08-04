using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Chronostianity", menuName = "ScriptableObjects/Readable Book", order = 1)]

public class Item_ReadableBook : ScriptableObject
{
    
    [System.Serializable]
    public class Chapter
    {
        public string chapterName = "I. INTRODUCTIONS";
        [TextArea(3,4)] public string content = " All the Powers of old Europe have entered into a holy alliance to exorcise this spectre: Pope and Czar, Metternich and Guizot, French Radicals and German police-spies.";
    }

    public string Title = "Communist Manifesto";
    public List<Chapter> allChapters = new List<Chapter>();

    public Chapter GetChapter(int index)
    {
        if (allChapters.Count < index)
            return null;

        if (index < 0)
            return null;

        return allChapters[index];
    }

}
