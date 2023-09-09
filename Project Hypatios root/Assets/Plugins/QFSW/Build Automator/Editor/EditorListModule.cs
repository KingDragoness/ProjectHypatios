using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QFSW.BA
{
    public abstract class EditorListModule<T>
    {
        public int Count { get; private set; }

        protected virtual int minCount { get { return 0; } }
        protected virtual int maxCount { get { return int.MaxValue; } }

        protected abstract GUIContent ListTitle { get; }
        protected abstract GUIContent ItemTitle { get; }
        protected abstract string DataName { get; }

        private readonly List<T> internalList = new List<T>();
        private readonly List<T> _internalList = new List<T>();

        protected const float miniButtonHeight = 16;
        protected const float miniButtonWidth = 17;

        private GUIStyle arrowStyle;
        private bool stylesCreated;

        private void CreateStyles()
        {
            if (!stylesCreated)
            {
                stylesCreated = true;

                arrowStyle = new GUIStyle();
                arrowStyle.alignment = TextAnchor.MiddleCenter;
                arrowStyle.padding = new RectOffset(-2, -2, -2, -4);
                arrowStyle.fontSize = 13;
            }
        }

        public void DrawList()
        {
            CreateStyles();
            GUI.enabled = true;
            DrawPreList();
            EditorGUILayout.BeginHorizontal();
            Count = EditorGUILayout.DelayedIntField(ListTitle, Count);
            DrawAdditionalHeader();

            GUI.enabled = Count > minCount;
            if (GUILayout.Button(new GUIContent("-", "Reduces the " + DataName + " count by 1."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(miniButtonWidth))) { Count--; }
            GUI.enabled = Count < maxCount;
            if (GUILayout.Button(new GUIContent("+", "Increases the " + DataName + " count by 1."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(miniButtonWidth))) { Count++; }
            EditorGUILayout.EndHorizontal();

            UpdateInternalLists();

            for (int i = 0; i < Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUIContent itemContent = new GUIContent(ItemTitle);
                if (i == 0) { itemContent.tooltip += "1."; }
                else
                {
                    itemContent.text = "";
                    itemContent.tooltip += i.ToString() + ".";
                }
                if (i == 0) { internalList[i] = DrawItemDisplay(itemContent, internalList[i]); }
                else { internalList[i] = DrawItemDisplay(new GUIContent(" ", "The preset name for preset " + (i + 1).ToString() + "."), internalList[i]); }

                if (internalList.Count > _internalList.Count) { _internalList.Add(internalList[i]); }
                _internalList[i] = internalList[i];

                GUI.enabled = i > 0 && Count > 0;
                //if (GUILayout.Button(new GUIContent("▲", "Moves element " + i.ToString() + " into position " + (i - 1).ToString() + ". "), arrowStyle, GUILayout.Height(miniButtonHeight), GUILayout.Width(miniButtonWidth))) { Swap(i, i - 1); }
                GUI.enabled = i < Count - 1;
                //if (GUILayout.Button(new GUIContent("▼", "Moves element " + i.ToString() + " into position " + (i + 1).ToString() + ". "), arrowStyle, GUILayout.Height(miniButtonHeight), GUILayout.Width(miniButtonWidth))) { Swap(i, i + 1); }
                GUI.enabled = Count > minCount;
                if (GUILayout.Button(new GUIContent("✕", "Removes the " + DataName + " from the list."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(miniButtonWidth))) { RemoveListItem(i); }

                EditorGUILayout.EndHorizontal();
            }

            GUI.enabled = true;
            DrawPostList();
        }

        protected abstract T DrawItemDisplay(GUIContent content, T data);

        protected virtual void DrawAdditionalHeader() { }
        protected virtual void DrawPreList() { }
        protected virtual void DrawPostList() { }

        private void RemoveListItem(int index)
        {
            if (index >= 0 && index < internalList.Count)
            {
                internalList.RemoveAt(index);
                if (index < _internalList.Count) { _internalList.RemoveAt(index); }
                Count--;
            }
        }

        protected void UpdateInternalLists()
        {
            Count = Mathf.Clamp(Count, minCount, maxCount);

            if (Count < internalList.Count)
            {
                internalList.RemoveRange(Count, internalList.Count - Count);
            }

            while (Count > internalList.Count)
            {
                if (internalList.Count >= _internalList.Count)
                {
                    internalList.Add(default(T));
                    _internalList.Add(default(T));
                }
                else { internalList.Add(_internalList[internalList.Count]); }
            }
        }

        private void Swap(int i, int j)
        {
            T tmp = internalList[i];
            internalList[i] = internalList[j];
            internalList[j] = tmp;
        }

        public T this[int index]
        {
            get { return internalList[index]; }
            set
            {
                internalList[index] = value;
                if (index < _internalList.Count) { _internalList[index] = value; }
            }
        }

        public void Resize(int size)
        {
            Count = size;
            UpdateInternalLists();
        }

        public void Add(T item)
        {
            Count++;
            internalList.Add(item);
            UpdateInternalLists();
        }

        public void RemoveAll(Predicate<T> condition)
        {
            internalList.RemoveAll(condition);
            Count = internalList.Count;
            UpdateInternalLists();
        }

        public bool Contains(T item) { return internalList.Contains(item); }
        public bool Exists(Predicate<T> condition) { return internalList.Exists(condition); }
    }
}
