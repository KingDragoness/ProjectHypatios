using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class BookChapterButton : MonoBehaviour
{

    public ReadableBookUI bookUI;
    public Text label;
    public int index = 0;


    public void OpenChapter()
    {
        bookUI.currentChapter = index;
        bookUI.UpdateUI();
    }

}
