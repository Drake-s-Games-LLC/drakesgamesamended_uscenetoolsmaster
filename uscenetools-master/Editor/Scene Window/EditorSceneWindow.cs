using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
#endif

namespace AdditiveSceneGroups.Editor {
    using Random = System.Random;

    public class EditorSceneWindow : EditorWindow, IEditorSceneManager {
        public ActiveScenesData ActiveScenes { get { return activeScenes; } }
        public SerializedObject SerializedManifest { get; private set; }
        public SerializedProperty SerializedSets { get; private set; }

        public SceneManifest Manifest => manifest;

        public event Action<ActiveScenesData> ActiveScenesChanged;
        public event Action<SceneManifest> SceneManifestChanged;

        [SerializeField] private SceneManifest manifest;

        private ActiveScenesData activeScenes;
        private Random rng;

        [MenuItem("Tools/AdditiveSceneGroups/Open Scene Editor Window")]
        public static void Open() {
            GetWindow<EditorSceneWindow>("Scene Workspace");
        }

        private void OnEnable() {
            rng = new Random();
            manifest = LoadSceneManifest();
            SerializedManifest = new SerializedObject(manifest);
            SerializedSets = SerializedManifest.FindProperty("sets");

            EditorSceneManager.sceneLoaded += OnSceneLoad;
            EditorSceneManager.sceneUnloaded += OnSceneUnloaded;
            EditorSceneManager.sceneOpened += OnSceneOpen;
            EditorSceneManager.sceneClosed += OnSceneUnloaded;

            Undo.undoRedoPerformed += NotifyManifestUpdate;

            var root = this.rootVisualElement;
            root.Add(new SceneGroupDisplay(this));
            var styleSheet = Resources.Load<StyleSheet>(
                EditorGUIUtility.isProSkin ? "AdditiveScenesWorkspaceDark" : "AdditiveScenesWorkspaceLight");
            root.styleSheets.Add(styleSheet);

            FetchActiveScenes();
        }

        private void OnDisable() {
            EditorSceneManager.sceneLoaded -= OnSceneLoad;
            EditorSceneManager.sceneUnloaded -= OnSceneUnloaded;
            EditorSceneManager.sceneOpened -= OnSceneOpen;
            EditorSceneManager.sceneClosed -= OnSceneUnloaded;

            Undo.undoRedoPerformed -= NotifyManifestUpdate;
        }

        public void NotifyManifestUpdate() {
            SceneManifestChanged?.Invoke(manifest);
        }

        public void SaveManifest() {
            var path = EditorUtility.SaveFilePanelInProject("Save Scene Manifest", "Scene Manifest", "asset", "Where to save?");
            if (!string.IsNullOrEmpty(path)) {
                AssetDatabase.CreateAsset(manifest, path);
                AssetDatabase.SaveAssets();

                manifest = AssetDatabase.LoadAssetAtPath<SceneManifest>(path);

                SerializedManifest = new SerializedObject(manifest);
                SerializedSets = SerializedManifest.FindProperty("sets");
                SceneManifestChanged?.Invoke(manifest);
            }
        }

        private void FetchActiveScenes() {
            var loadedScenes = EditorSceneUtility.GetLoadedScenes();
            activeScenes = new ActiveScenesData {
                Scenes = loadedScenes,
                Hash = rng.Next()
            };

            ActiveScenesChanged?.Invoke(activeScenes);
        }

        /// <summary>
        /// Loads the first instance of a Scene Bank from the project. If there is none, 
        /// then a virtual instance is created
        /// </summary>
        private SceneManifest LoadSceneManifest() {
            if (manifest != null) { return manifest; }

            var guids = AssetDatabase.FindAssets("t:SceneManifest");
            if (guids.Length > 0) {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<SceneManifest>(path);
            } else {
                return CreateInstance<SceneManifest>();
            }
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
            FetchActiveScenes();
            Repaint();
        }

        private void OnSceneOpen(Scene scene, OpenSceneMode mode) {
            FetchActiveScenes();
            Repaint();
        }

        private void OnSceneUnloaded(Scene scene) {
            FetchActiveScenes();
            Repaint();
        }


    }
}
