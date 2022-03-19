using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using tripolygon.UModeler;

namespace TPUModelerEditor
{
    [InitializeOnLoadAttribute]
    public static class UModelerEditorInitializer
    {
        static UModelerEditorInitializer()
        {
            UMContext.Init(new EditorEngine());

            Selection.selectionChanged += HandleOnSelectionChanged;
            Builder.callback += OnMeshBuilt;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneLoaded;
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaving;
            PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;

#if UNITY_2019_2_OR_NEWER
            Lightmapping.bakeStarted += OnLightmapBake;
#endif

#if UNITY_2019_3_OR_NEWER
            EditorDecl.SettingsBoxHeight = 280;
#else
            EditorDecl.SettingsBoxHeight = 255;
#endif

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#else
            EditorApplication.playmodeStateChanged += HandleOnPlayModeChanged;
#endif

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += UModelerEditor.OnScene;
#else
            SceneView.onSceneGUIDelegate += UModelerEditor.OnScene;
#endif
        }

        private static void OnLightmapBake()
        {
            EditorUtil.RefreshAll(/*bOutputLog */false);
        }

        public static void HandleOnSelectionChanged()
        {
            UModelerEditor.SendMessage(UModelerMessage.SelectionChanged);

            if (UMContext.activeModeler != null)
            {
                EditorMode.commentaryViewer.AddTitleNoDuplilcation("[" + UMContext.activeModeler.gameObject.name + "] Object has been selected.");
            }
        }

#if UNITY_2017_2_OR_NEWER
        public static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (EditorMode.currentTool != null && UMContext.activeModeler != null)
                {
                    EditorMode.currentTool.End();
                    EditorMode.currentTool.Start();
                }

                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponent<UModeler>() != null)
                    {
                        Selection.activeGameObject = null;
                    }
                }

                UModeler.enableDelegate = true;
            }

            if (state == PlayModeStateChange.ExitingEditMode)
            {
                UModeler.enableDelegate = false;
            }

            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                EditorUtil.RefreshAll(false/*bOutputLog*/);
                EditorUtil.DisableHasTransformed();
            }
        }
#else
        public static void HandleOnPlayModeChanged()
        {
            bool bExitingEditMode = !EditorApplication.isPlaying &&  EditorApplication.isPlayingOrWillChangePlaymode;
            bool bEnteredEditMode = !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;
            bool bEnteredPlayMode =  EditorApplication.isPlaying &&  EditorApplication.isPlayingOrWillChangePlaymode;

            if (bExitingEditMode)
            {
                UModeler.enableDelegate = true;
                if (EditorMode.currentTool != null && UMContext.activeModeler != null)
                {
                    EditorMode.currentTool.End();
                    EditorMode.currentTool.Start();
                }

                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponent<UModeler>() != null)
                    {
                        Selection.activeGameObject = null;
                    }
                }
            }

            if (bEnteredPlayMode)
            {
                UModeler.enableDelegate = false;
            }

            if (bEnteredPlayMode || bEnteredEditMode)
            {
                EditorUtil.RefreshAll(false/*bOutputLog*/);
                EditorUtil.DisableHasTransformed();
            }
        }
#endif

        static void OnSceneLoaded(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            EditorUtil.DisableHasTransformed();
            MenuGUICacheData.Invalidate();
            Selection.activeObject = null;
        }

        static void OnSceneSaving(Scene scene, string path)
        {
            if (UMContext.activeModeler != null && EditorMode.currentTool != null)
            {
                EditorMode.currentTool.End();
                EditorMode.currentTool.Start();

                if (!UMContext.activeModeler.editableMesh.IsEmpty(1))
                {
                    UMContext.activeModeler.editableMesh.Clear(1);
                    UMContext.activeModeler.Build(1);
                }
            }
        }

        static void OnMeshBuilt(UModeler modeler, int shelf)
        {
            UModelerEditor.OnChanged();
            EditorUtil.EnableHighlight(false, modeler);

            if (shelf == 0)
            {
                EditorUtility.SetDirty(modeler);
                if (EditorUtil.HasStaticLightmap(modeler))
                {
                    EditorUtil.SetLightmap(modeler, false);
                    EditorUtil.SetLightmap(modeler, true);
                }
            }
        }

        static public void PrefabInstanceUpdated(GameObject instance)
        {
            UpdateInstance(instance);
        }

        static private void UpdateInstance(GameObject instance)
        {
            UModeler modeler = instance.GetComponent<UModeler>();
            if (modeler != null)
            {
                modeler.editableMesh.InvalidateCache();
                using (new ActiveModelerHolder(modeler))
                {
                    modeler.Build(0, /*updateToGraphicsAPIImmediately*/true);
                }
            }

            for (int i = 0; i < instance.transform.childCount; ++i)
            {
                UpdateInstance(instance.transform.GetChild(i).gameObject);
            }
        }
    }
}