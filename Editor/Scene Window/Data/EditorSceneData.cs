using System;

namespace AdditiveSceneGroups.Editor {
    [Serializable]
    public struct EditorSceneData {
        public string name;
        public bool isMainScene;
        public string path;

        public static implicit operator SceneData(EditorSceneData data) {
            return new SceneData {
                Name = data.name,
                IsMainScene = data.isMainScene
            };
        }
    }
}
