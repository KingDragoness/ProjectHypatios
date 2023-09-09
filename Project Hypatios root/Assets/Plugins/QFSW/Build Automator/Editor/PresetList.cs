using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QFSW.BA
{
    public class PresetList : EditorListModule<string>
    {
        protected override GUIContent ListTitle { get { return new GUIContent("Preset Count", "Number of presets to include in the automation cycle."); } }
        protected override GUIContent ItemTitle { get { return new GUIContent("Presets:", "The preset name for preset "); } }
        protected override string DataName { get { return "preset"; } }

        private int currentPresetID = 0;
        private string[] presets = new string[0];

        public void UpdateData(string[] presets, int currentPresetID)
        {
            this.presets = presets;
            this.currentPresetID = currentPresetID;
        }

		protected override string DrawItemDisplay(GUIContent content, string data)
		{
            return EditorGUILayout.TextField(content, data);
		}

		protected override void DrawAdditionalHeader()
		{
			base.DrawAdditionalHeader();

            GUI.enabled = presets.Length > 1;
            if (GUILayout.Button(new GUIContent("Add All Presets", "Adds all of the presets in this project to the pipeline."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(90))) { AddAllPresets(); }
            GUI.enabled = presets.Length > 1 && currentPresetID > 0;
            if (GUILayout.Button(new GUIContent("Add Current Preset", "Adds the current preset to the list of extra scenes."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(110))) { AddCurrentPreset(); }
		}

        private void AddCurrentPreset()
        {
            if (!Contains(presets[currentPresetID]))
            {
                Add(presets[currentPresetID]);
            }
        }

        private void AddAllPresets()
        {
            for (int i = 1; i < presets.Length; i++)
            {
                if (!Contains(presets[i]))
                {
                    Add(presets[i]);
                }
            }
        }
	}
}
