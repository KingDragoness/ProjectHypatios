#if UNITY_EDITOR

/* 

  Asset Selection History
  @polemical, Unity Forum

  2020-08-20 - 1.0.2

  - Fixed
    - Different projects would use the same history.

  2020-08-20 - 1.0.1

  - Fixed
    - Entering Play Mode would result in the History being lost.

  - Added
    - History is now saved to EditorPrefs and restored when reopening Unity or as necessary.
    - Ability to reselect the indicated selection (i.e. for after deselection).
    - Ability to remove current selection from the list.

  - Changed
    - The button that previously was used to clear the history now opens a menu.
    - Selecting the same objects as a selection already added to history will 
      now re-select that in the list instead of adding a duplicate, up to LIMIT_DUPLICATES 
      number of most recent selections, or 0 (allow), -1 (check all).
      - By default, this only checks for duplicates in the 100 most recent selections.

  2020-08-19 - Initial Release
  
  - Inspired by https://forum.unity.com/threads/can-we-get-a-back-button-in-the-project-panel.723560/

*/

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class AssetSelectionHistory : EditorWindow
{

  private static string ICON_PATH = "Assets/AssetSelectionHistory/Editor/Icons/";

  private static string ICON_BACK = ICON_PATH + "ash_back.png";
  private static string ICON_FORWARD = ICON_PATH + "ash_forward.png";
  private static string ICON_OPTIONS = ICON_PATH + "ash_options.png";

  private static int LIMIT_DUPLICATES = 100; // 0: disable, -1: check all, otherwise up to # most recent.

  private static float BUTTON_SIZE = 24.0f;

  private static AssetSelectionHistory window;

  private static Texture2D[] icons = new Texture2D[3];

  private static GUIStyle labelStyle;
  private static GUIStyle buttonStyle;
  private static GUIStyle optionsButtonStyle;
  private static GUIStyle dropdownStyle;

  private List<Object[]> history;
  private List<string> guidsEncoded;
  private List<string> alias;

  private int historyIndex = -1;
  private bool selectionChangedByEditor = false;

  [MenuItem("Tools/Asset Selection History")]
  public static void OpenWindow()
  {
    window = GetWindow<AssetSelectionHistory>();
    window.titleContent.text = "Asset Selection History";
    window.minSize = new Vector2(180, BUTTON_SIZE);
    window.ResetHistory();
    if (Selection.assetGUIDs.Length != 0) window.AddHistory(Selection.objects);
    window.ShowTab();
  }
  void ResetHistory(bool clear = false)
  {
    if (clear)
    {
      historyIndex = -1;
      if (history == null) { history = new List<Object[]>(); } else { history.Clear(); }
      if (alias == null) { alias = new List<string>(); } else { alias.Clear(); }
      if (guidsEncoded == null) { guidsEncoded = new List<string>(); } else { guidsEncoded.Clear(); }
      SaveHistory();
    } else LoadHistory();
  }
  void AddHistory(Object[] selection)
  {
    if ((Selection.assetGUIDs == null) || (Selection.assetGUIDs.Length == 0)) return;
    if ((selection == null) || (selection.Length == 0)) return;
    if (history == null)
    {
      history = new List<Object[]>();
      alias = new List<string>();
      guidsEncoded = new List<string>();
      historyIndex = -1;
    }

    string a = selection[0].name;
    if (selection.Length > 1) a += " +" + (selection.Length - 1);
    if (history.Count != 0)
    {
      int hmax = history.Count - 1;
      int h = hmax;
      if (history[h][0] == selection[0])
      {
        // add to latest selection
        history[h] = selection;
        guidsEncoded[h] = GUIDsFromSelection(h);
        alias[h] = (h + 1) + ". " + a;
        historyIndex = h;
        SaveHistory();
        return;
      }
      if (LIMIT_DUPLICATES != 0)
      {
        int hmin = (LIMIT_DUPLICATES == -1) ? -1 : Mathf.Max(-1, history.Count - (LIMIT_DUPLICATES + 2));
        for (h = hmax - 1; h > hmin; h--)
        {
          if (history[h].Length == selection.Length)
          {
            bool same = true;
            for (int i = 0; i < history[h].Length; i++)
            {
              if (history[h][i] != selection[i])
              {
                same = false;
                break;
              }
            }
            if (same)
            {
              historyIndex = h;
              Repaint();
              return;
            }

          }
        }
      }
    }
    a = (alias.Count + 1) + ". " + a;

    alias.Add(a);
    historyIndex = alias.Count - 1;
    history.Add(selection);
    guidsEncoded.Add(GUIDsFromSelection(historyIndex));
    SaveHistory();
  }
  string GUIDsFromSelection(int index)
  {
    // note: can't just return joined Selection.assetGUIDs because objects will not always be in the same order as selected.
    Object[] selection = history[index];
    List<string> result = new List<string>();
    for (int o = 0; o < selection.Length; o++)
    {
      string assetPath = AssetDatabase.GetAssetPath(selection[o]);
      result.Add(((assetPath == null) || (assetPath.Length == 0)) ? "" : AssetDatabase.AssetPathToGUID(assetPath));
    }
    return string.Join<string>("*", result);
  }
  void SaveHistory()
  {
    EditorPrefs.SetString("ash_" + Application.dataPath, string.Join<string>("?", guidsEncoded));
  }
  void LoadHistory()
  {
    string value = EditorPrefs.GetString("ash_" + Application.dataPath, "");
    if (value.Length == 0)
    {
      ResetHistory(true);
      return;
    }
    
    guidsEncoded = new List<string>(value.Split('?'));
    int count = guidsEncoded.Count;
    alias = new List<string>();
    history = new List<Object[]>();
    for (int e = 0; e < count; e++)
    {
      string[] guids = guidsEncoded[e].Split('*');
      Object[] selection = new Object[guids.Length];
      for (int o = 0; o < selection.Length; o++)
      {
        selection[o] = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guids[o]));
      }
      history.Add(selection);
      string a = (e + 1).ToString() + ". " + ((selection[0] == null) ? "(missing)" : selection[0].name);
      if (selection.Length > 1) a += " +" + (selection.Length - 1);
      alias.Add(a);
    }
    historyIndex = history.Count - 1;
    Repaint();
  }
  void OnSelectionChange()
  {
    Object[] currentSelection = Selection.objects;
    if (selectionChangedByEditor)
    {
      if ((currentSelection != null) && (currentSelection.Length > 0)) EditorGUIUtility.PingObject(currentSelection[0]);
      selectionChangedByEditor = false;
      EditorUtility.FocusProjectWindow();
      return;
    }
    AddHistory(currentSelection);
    Repaint();
  }
  void OnGUI()
  {
    CheckStyles();
    if (history == null) ResetHistory();
    if ((historyIndex == -1) || (historyIndex >= history.Count))
    {
      historyIndex = -1;
      EditorGUILayout.LabelField("Select Asset(s)", labelStyle);
      return;
    }

    CheckIcons();
    float rightX = EditorGUIUtility.currentViewWidth;
    if (GUI.Button(new Rect(rightX - (BUTTON_SIZE * 2), 0, BUTTON_SIZE, BUTTON_SIZE), icons[0], buttonStyle))
    {
      if (historyIndex > 0) historyIndex--;
      selectionChangedByEditor = true;
      Selection.objects = history[historyIndex];
    }
    if (GUI.Button(new Rect(rightX - BUTTON_SIZE, 0, BUTTON_SIZE, BUTTON_SIZE), icons[1], buttonStyle))
    {
      if (historyIndex < (history.Count - 1)) historyIndex++;
      selectionChangedByEditor = true;
      Selection.objects = history[historyIndex];
    }
    if (GUI.Button(new Rect(0, 0, BUTTON_SIZE, BUTTON_SIZE), icons[2], optionsButtonStyle))
    {
      GenericMenu menu = new GenericMenu();
      menu.AddItem(new GUIContent("Reselect " + alias[historyIndex]), false, ReselectMenuItemSelected);
      menu.AddItem(new GUIContent("Remove From History"), false, RemoveFromListMenuItemSelected);
      menu.AddSeparator("");
      menu.AddItem(new GUIContent("Reset History"), false, ClearHistoryMenuItemSelected);
      menu.DropDown(new Rect(0, BUTTON_SIZE - 3, 0, 0));
    }
    int historyIndexPrev = historyIndex;
    dropdownStyle.fixedWidth = rightX - (BUTTON_SIZE * 3);
    historyIndex = EditorGUILayout.Popup(historyIndex, alias.ToArray(), dropdownStyle);
    if (historyIndex != historyIndexPrev)
    {
      selectionChangedByEditor = true;
      Selection.objects = history[historyIndex];
    }

  }
  void ReselectMenuItemSelected()
  {
    selectionChangedByEditor = true;
    Selection.objects = history[historyIndex];
  }
  void RemoveFromListMenuItemSelected()
  {
    if (history.Count == 1)
    {
      ResetHistory(true);
      if (Selection.assetGUIDs.Length != 0)
      {
        if (EditorUtility.DisplayDialog("Remove From History", "Last item removed. Add currently-selected assets?", "Yes", "No"))
        {
          AddHistory(Selection.objects);
        }
      }
      Repaint();
      return;
    }
    history.RemoveAt(historyIndex);
    alias.RemoveAt(historyIndex);
    guidsEncoded.RemoveAt(historyIndex);
    if (historyIndex >= history.Count) historyIndex = history.Count - 1;
    for (int i = historyIndex; i < history.Count; i++)
    {
      alias[i] = (i + 1) + ". " + ((history[i][0] == null) ? "(missing)" : history[i][0].name);
      if (history[i].Length > 1) alias[i] += " +" + (alias[i].Length - 1);
    }
    if (Selection.assetGUIDs.Length == 0) return;
    selectionChangedByEditor = true;
    Selection.objects = history[historyIndex];
    SaveHistory();
  }
  void ClearHistoryMenuItemSelected()
  {
    if (EditorUtility.DisplayDialog("Clear History", (Selection.assetGUIDs.Length != 0) ? "Set history to the current selection only?" : "Are you sure?", "OK", "Cancel"))
    {
      ResetHistory(true);
      AddHistory(Selection.objects);
    }
  }
  static void CheckStyles()
  {
    if (labelStyle == null) labelStyle = new GUIStyle(EditorStyles.boldLabel)
    {
      alignment = TextAnchor.MiddleCenter
    };
    if (buttonStyle == null) buttonStyle = new GUIStyle(EditorStyles.miniButton)
    {
      padding = new RectOffset(3, 3, 3, 3),
      margin = new RectOffset(0, 0, 0, 0),
      fixedHeight = BUTTON_SIZE,
      fixedWidth = BUTTON_SIZE
    };
    if (optionsButtonStyle == null) optionsButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
    {
      padding = new RectOffset(1, 1, 1, 1),
      margin = new RectOffset(0, 0, 0, 0),
      fixedHeight = BUTTON_SIZE,
      fixedWidth = BUTTON_SIZE
    };
    if (dropdownStyle == null) dropdownStyle = new GUIStyle(EditorStyles.toolbarDropDown)
    {
      fixedHeight = BUTTON_SIZE,
      margin = new RectOffset((int)BUTTON_SIZE, 0, 0, 0)
    };
  }
  static void CheckIcons()
  {
    if (icons[0] == null) icons[0] = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture>(ICON_BACK);
    if (icons[1] == null) icons[1] = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture>(ICON_FORWARD);
    if (icons[2] == null) icons[2] = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture>(ICON_OPTIONS);
  }

}
#endif