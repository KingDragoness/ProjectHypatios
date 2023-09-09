using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace QFSW.BA
{
    public class SceneList : EditorListModule<SceneAsset>
    {
        public bool ValidSceneList
        {
            get
            {
                if (useEditorScenes) { return EditorBuildSettings.scenes.Length > 0; }
                else if (Count == 0) { return false; }
                else { return Exists(x => x != null); }
            }
        }

        protected override GUIContent ListTitle { get { return new GUIContent("Scene Count", "Number of scenes to include in the build."); } }
        protected override GUIContent ItemTitle { get { return new GUIContent("Scenes:", "The SceneAsset for scene "); } }
        protected override string DataName { get { return "scene"; } }
        protected override int minCount { get { return 1; } }

        public bool useEditorScenes;

        protected override void DrawPreList()
        {
            GUILayout.BeginHorizontal();
            useEditorScenes = EditorGUILayout.Toggle(new GUIContent("Ignore Scene Settings", "Ignores the scene settings chosen in the Build Automator window and uses the scenes selected in Unity's built in Build Settings window instead."), useEditorScenes);
            GUI.enabled = !useEditorScenes;
            if (GUILayout.Button(new GUIContent("Import Editor Scene List", "Imports the scene list from the editor's built in Build Settings window."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(135))) { ImportEditorScenes(); }
            GUILayout.EndHorizontal();
        }

        protected override void DrawAdditionalHeader()
		{
            if (GUILayout.Button(new GUIContent("Add All Scenes", "Adds all of the scenes in this project to the list of scenes."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(90))) { AddAllScenes(); }
            if (GUILayout.Button(new GUIContent("Add Current Scene", "Add the current scene to the list of scenes."), EditorStyles.miniButton, GUILayout.Height(miniButtonHeight), GUILayout.Width(105))) { AddCurrentScene(); }
        }

        protected override SceneAsset DrawItemDisplay(GUIContent content, SceneAsset data)
		{
            return (SceneAsset)EditorGUILayout.ObjectField(content, data, typeof(SceneAsset), false);
		}

        private void AddCurrentScene()
        {
            SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
            if (!Contains(currentScene)) { Add(currentScene); }
            RemoveAll(x => x == null);
        }

        private void AddAllScenes()
        {
            RemoveAll(x => x == null);
            string[] sceneGUIDs = AssetDatabase.FindAssets("t:SceneAsset");
            for (int i = 0; i < sceneGUIDs.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUIDs[i]);
                SceneAsset newScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (!Contains(newScene)) { Add(newScene); }
            }

            RemoveAll(x => x == null);
        }

        private bool ImportEditorScenes()
        {
            string[] scenePaths;
#if !UNITY_5_6_OR_NEWER
            scenePaths = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
#else
            List<EditorBuildSettingsScene> sceneObjects = EditorBuildSettings.scenes.ToList();
            sceneObjects.RemoveAll((EditorBuildSettingsScene x) => !x.enabled);
            scenePaths = new string[sceneObjects.Count];
            for (int i = 0; i < scenePaths.Length; i++) { scenePaths[i] = sceneObjects[i].path; }
#endif
            if (scenePaths.Length > 0)
            {
                Resize(scenePaths.Length);
                for (int i = 0; i < scenePaths.Length; i++) { this[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePaths[i]); }
                return true;
            }
            else { return false; }
        }
    }
}
