using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// A custom inspector for the scene loader
    /// </summary>
    [CustomEditor (typeof (SceneLoader))]
    public class SceneLoaderInspector : UnityEditor.Editor {

        private void OnEnable () {
            EditorSceneManager.sceneLoaded += OnSceneChange;
            EditorSceneManager.sceneUnloaded += OnSceneChange;
        }

        private void OnDisable () {
            EditorSceneManager.sceneLoaded -= OnSceneChange;
            EditorSceneManager.sceneUnloaded -= OnSceneChange;
        }

        public override void OnInspectorGUI () {
            base.OnInspectorGUI ();

            DisplayActiveScenes (target as SceneLoader);
        }

        public override bool RequiresConstantRepaint () {
            return EditorApplication.isPlayingOrWillChangePlaymode;
        }

        /// <summary>
        /// Draws a list of actively loaded scenes in the editor (Playmode Only)
        /// </summary>
        private void DisplayActiveScenes (SceneLoader loader) {
            var scenes = loader.ActiveScenes;

            EditorGUILayout.LabelField (string.Format ("Active Scenes: {0}", scenes.Length),
                EditorStyles.boldLabel);

            var sceneStyle = EditorStyles.largeLabel;
            for (var i = 0; i < scenes.Length; ++i) {
                EditorGUILayout.LabelField (scenes[i], sceneStyle);
            }
        }

        private void OnSceneChange (Scene scene) => Repaint ();
        private void OnSceneChange (Scene scene, LoadSceneMode mode) => Repaint ();
    }
}
