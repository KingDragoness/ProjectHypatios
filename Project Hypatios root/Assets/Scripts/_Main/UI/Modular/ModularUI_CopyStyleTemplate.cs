using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ModularUI_CopyStyleTemplate : MonoBehaviour
{
    
    public List<UIBehaviour> allUIBehaviours = new List<UIBehaviour>();
    public GameObject copyTarget;
    public bool initiateOnStart = false;

    private List<UIBehaviour> target_UIElements = new List<UIBehaviour>();

    private void Start()
    {
        if (initiateOnStart == true)
        {
            Test_Paste();
        }
    }


   // [FoldoutGroup("DEBUG")] [Button("Paste-Test")] doesn't work
    public void Test_Paste()
    {
        CollectComponents();

        for (int x = 0; x < allUIBehaviours.Count; x++)
        {
            Graphic graphic_a = allUIBehaviours[x] as Graphic;
            Selectable selectable_a = allUIBehaviours[x] as Selectable;
            Text text_a = allUIBehaviours[x] as Text;
            Graphic graphic_b = target_UIElements[x] as Graphic;
            Selectable selectable_b = target_UIElements[x] as Selectable;
            Text text_b = target_UIElements[x] as Text;
            bool notExist = true;

            if (graphic_a != null)
            {
                notExist = false;
                graphic_a.color = graphic_b.color;

            }

            if (selectable_a != null)
            {
                notExist = false;
                selectable_a.colors = selectable_b.colors;


            }

            if (text_a != null)
            {
                notExist = false;
                text_a.font = text_b.font;

            }

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                Undo.RecordObject(graphic_a, "Changed Color");
                Undo.RecordObject(selectable_a, "Changed Color");
                Undo.RecordObject(text_a, "Changed Color");
            }

            #endif


            if (notExist)
            {
                Debug.LogWarning($"{target_UIElements[x].gameObject.name} has no valid element.");
            }
        }
    }

    private void CollectComponents()
    {
        target_UIElements = copyTarget.GetComponentsInChildren<UIBehaviour>(true).ToList();
        allUIBehaviours = gameObject.GetComponentsInChildren<UIBehaviour>(true).ToList();

        if (target_UIElements.Count != allUIBehaviours.Count)
        {
            Debug.LogError("Failed. Target copy is not same object!");
            return;
        }


        for (int x = 0; x < allUIBehaviours.Count; x++)
        {
            var element = allUIBehaviours[x];
            var similarElement = target_UIElements[x];
            string s_typename = element.GetType().Name;
            string s_typename1 = similarElement.GetType().Name;

            if (s_typename != s_typename1)
            {
                Debug.LogError("Failed. Target copy is not same object!");
                return;
            }

        }
    }

}
