using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// The primary display for showing active scenes and saved groups
    /// </summary>
    public class SceneGroupDisplay : VisualElement {

        private IEditorSceneManager sceneManager;
        private ReorderableList sceneGroupList;
        private ActiveScenesWidget activeSceneWidget;

        public SceneGroupDisplay(IEditorSceneManager sceneManager) {
            this.sceneManager = sceneManager;

            this.Add(new IMGUIContainer(DrawBankWarnings));
            this.Add(activeSceneWidget = new ActiveScenesWidget(sceneManager));
            this.Add(new SceneGroupListWidget(sceneManager));
        }

        private void DrawBankWarnings() {
            if (!AssetDatabase.IsNativeAsset(sceneManager.Manifest)) {
                EditorGUILayout.HelpBox(
                    "There is no Scene Bank in the project!  Changes here will be temporary!",
                    MessageType.Warning
                );

                if (GUILayout.Button("Click here to create a persistent bank")) {
                    sceneManager.SaveManifest();
                }
            }
        }
    }
}
