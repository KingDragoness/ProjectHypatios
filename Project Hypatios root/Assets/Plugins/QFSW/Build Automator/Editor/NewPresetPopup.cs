using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace QFSW.BA
{
    public partial class BuildAutomator
    {
        /// <summary>Popwindow that creates new presets for a Build Automator.</summary>
        private class PresetPopup : PopupWindowContent
        {
            /// <summary>The Build Automator window that this popup window belongs to.</summary>
            public BuildAutomator ParentBA;

            /// <summary>The name of the preset to be created.</summary>
            string PresetName;
            /// <summary>Any errors that were encountered whilst creating the preset.</summary>
            string Errors;
            /// <summary>Message to indicate successful preset creation.</summary>
            string SuccessMessage;

            //Custom GUIStyles
            /// <summary>GUIStyle for error messages.</summary>
            private GUIStyle ErrorStyle;
            /// <summary>GUIStyle for success messages.</summary>
            private GUIStyle SuccessStyle;

            /// <summary>Constructs a new popup window for creating presets.</summary>
            /// <param name="ParentBA">The Build Automator window that this popup window belongs to.</param>
            public PresetPopup(BuildAutomator ParentBA)
            {
                this.ParentBA = ParentBA;
                CreateStyles();
            }

            /// <summary>Creates the custom editor styles.</summary>
            private void CreateStyles()
            {
                //Creates error style
                ErrorStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                ErrorStyle.normal.textColor = new Color(1, 0, 0);

                //Creates success style
                SuccessStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                SuccessStyle.normal.textColor = new Color(0, 0.5f, 0);
            }

            //Forces window size
            public override Vector2 GetWindowSize() { return new Vector2(300, 100); }

            //Draws window
            public override void OnGUI(Rect DrawRect)
            {
                //Gets name
                PresetName = EditorGUILayout.TextField(PresetName);

                //Create preset button
                GUI.enabled = !(String.IsNullOrEmpty(PresetName) || String.IsNullOrEmpty(PresetName.Trim()));
                if (GUILayout.Button("Create Preset")) { CreatePreset(PresetName); }

                //Displays error/success message
                if (String.IsNullOrEmpty(Errors)) { EditorGUILayout.LabelField(SuccessMessage, SuccessStyle); }
                else { EditorGUILayout.LabelField(Errors, ErrorStyle); }
            }

            /// <summary>Creates a new preset.</summary>
            /// <param name="DesiredName">The desired name of the preset.</param>
            private void CreatePreset(string DesiredName)
            {
                Errors = ParentBA.CreateNewPreset(DesiredName);
                if (String.IsNullOrEmpty(Errors)) { SuccessMessage = "Preset '" + DesiredName + "' was added successfully."; }
            }
        }
    }
}
