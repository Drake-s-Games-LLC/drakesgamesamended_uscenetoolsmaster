using UnityEditor.SceneManagement;

namespace AdditiveSceneGroups.Editor {
    public static class EditorSceneUtility {

        public static EditorSceneData[] GetLoadedScenes() {
            var scenes = new EditorSceneData[EditorSceneManager.loadedSceneCount];
            var activeScene = EditorSceneManager.GetActiveScene();

            for (var i = 0; i < scenes.Length; ++i) {
                var scene = EditorSceneManager.GetSceneAt(i);

                scenes[i] = new EditorSceneData {
                    name = scene.name,
                    isMainScene = scene == activeScene,
                    path = scene.path
                };
            }

            return scenes;
        }

        public static void LoadSceneGroup(SceneSet group) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            var scenes = group.Scenes;

            for (var i = 0; i < scenes.Length; ++i) {
                var scene = scenes[i];
                EditorSceneManager.OpenScene(scene.Path, i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);

                if (scene.IsMainScene) {
                    var activeScene = EditorSceneManager.GetSceneByName(scene.Name);
                    EditorSceneManager.SetActiveScene(activeScene);
                }
            }
        }

    }
}
