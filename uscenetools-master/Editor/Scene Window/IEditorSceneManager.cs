using System;
using UnityEditor;

namespace AdditiveSceneGroups.Editor {
    public interface IEditorSceneManager {
        ActiveScenesData ActiveScenes { get; }
        SceneManifest Manifest { get; }
        SerializedObject SerializedManifest { get; }
        SerializedProperty SerializedSets { get; }

        event Action<ActiveScenesData> ActiveScenesChanged;
        event Action<SceneManifest> SceneManifestChanged;

        void NotifyManifestUpdate();
        void Repaint();
        void SaveManifest();
    }
}
