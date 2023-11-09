using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

public class ReadableBookUI : MonoBehaviour
{

    public Text label_MainTitle;
    public RectTransform parentTextContent;
    public RectTransform parentBookButton;
    public Text prefab_TextEntry;
    public BookChapterButton prefab_ChapterBook;
    public Item_ReadableBook currentBook;
    public int fontSize_Subchapter = 22;
    public int currentChapter = -1;

    [ShowInInspector] private List<Text> allTextEntries = new List<Text>();
    [ShowInInspector] private List<BookChapterButton> allChapterButtons = new List<BookChapterButton>();


    private void Start()
    {
        prefab_TextEntry.gameObject.SetActive(false);
        prefab_ChapterBook.gameObject.SetActive(false);
    }

    public void OpenText(Item_ReadableBook _readableBook)
    {
        gameObject.SetActive(true);
        currentBook = _readableBook;
        UpdateUI();

    }

    public void UpdateUI()
    {
        label_MainTitle.text = currentBook.Title.ToUpper();
        Item_ReadableBook.Chapter chapter = currentBook.GetChapter(currentChapter);

        foreach(var entry in allTextEntries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }

        foreach (var button1 in allChapterButtons)
        {
            if (button1 != null)
                Destroy(button1.gameObject);
        }

        allTextEntries.Clear();
        allChapterButtons.Clear();

        if (chapter != null)
        {
            string newContent1 = $"<size={fontSize_Subchapter.ToString()}><b>{chapter.chapterName}</b></size>\n\n" + chapter.content;
            string[] lines = newContent1.Split(
                new string[] { System.Environment.NewLine }, StringSplitOptions.None
            );
            Debug.Log($"{lines.Length}");

            foreach(var line in lines)
            {
                Text t = Instantiate(prefab_TextEntry, parentTextContent);
                t.gameObject.SetActive(true);
                t.text = $"{line}\n";
                allTextEntries.Add(t);
            }
        }

        int index = 0;

        foreach(var chapterEntry in currentBook.allChapters)
        {
            BookChapterButton newButton = Instantiate(prefab_ChapterBook, parentBookButton);
            newButton.gameObject.SetActive(true);
            newButton.index = index;
            newButton.bookUI = this;
            newButton.label.text = $"{chapterEntry.chapterName}";
            allChapterButtons.Add(newButton);
            index++;
        }
    }

}
