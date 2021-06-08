using UnityEngine.UIElements;

namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// Diplays a list of active scenes
    /// </summary>
    public class ActiveScenesWidget : VisualElement {

        private IEditorSceneManager sceneManager;
        private ActiveScenesData currentScenes;

        private Label[] sceneLabels;
        private Button saveButton;

        public ActiveScenesWidget(IEditorSceneManager sceneManager) {
            this.sceneManager = sceneManager;
            sceneManager.ActiveScenesChanged += UpdateScenes;

            // Create a title label
            var buttonRow = new VisualElement { name = "group-list-title" };
            buttonRow.Add(new Label("Active Scenes") { name = "inspector-heading" });
            this.Add(buttonRow);

            saveButton = new Button(AddSceneGroup) { text = "Save As Group" };
            this.Add(saveButton);

            sceneLabels = new Label[0];
        }

        public void UpdateScenes(ActiveScenesData sceneData) {
            if (currentScenes.Hash != sceneData.Hash) {
                currentScenes = sceneData;

                DiscardOldLabels(sceneLabels);
                CreateSceneLabels(sceneData.Scenes, ref sceneLabels);

                // Remove and read the save button
                this.Remove(saveButton);
                this.Add(saveButton);
            }
        }

        private void AddSceneGroup() {
            sceneManager.SerializedManifest.Update();
            var arr = sceneManager.SerializedSets;
            arr.InsertArrayElementAtIndex(arr.arraySize);

            var element = arr.GetArrayElementAtIndex(arr.arraySize - 1);
            element.FindPropertyRelative("Name").stringValue = "New Scene Group";

            var sceneArr = element.FindPropertyRelative("Scenes");
            sceneArr.ClearArray();
            for (var i = 0; i < currentScenes.Scenes.Length; ++i) {
                var scene = currentScenes.Scenes[i];
                sceneArr.InsertArrayElementAtIndex(sceneArr.arraySize);

                var data = sceneArr.GetArrayElementAtIndex(sceneArr.arraySize - 1);
                data.FindPropertyRelative("Name").stringValue = scene.name;
                data.FindPropertyRelative("IsMainScene").boolValue = scene.isMainScene;
                data.FindPropertyRelative("Path").stringValue = scene.path;
            }

            sceneManager.SerializedManifest.ApplyModifiedProperties();
            sceneManager.NotifyManifestUpdate();
            sceneManager.Repaint();
        }

        private void CreateSceneLabels(EditorSceneData[] scenes, ref Label[] sceneLabels) {
            sceneLabels = new Label[scenes.Length];
            for (var i = 0; i < sceneLabels.Length; ++i) {
                var scene = scenes[i];

                var label = new Label();
                label.text = scene.isMainScene ? $"{scene.name} (Active)" : scene.name;
                if (scene.isMainScene) {
                    label.AddToClassList("main-scene");
                }

                sceneLabels[i] = label;
                this.Add(label);
            }
        }

        private void DiscardOldLabels(Label[] labels) {
            for (var i = 0; i < labels.Length; ++i) {
                this.Remove(labels[i]);
            }
        }
    }
}
