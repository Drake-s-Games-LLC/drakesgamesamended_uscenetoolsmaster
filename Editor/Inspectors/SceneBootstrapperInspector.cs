using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// A custom inspector for the SceneBootstrapper. Includes a reoderable list
    /// to allow quick reordering of scenes to load
    /// </summary>
    [CustomEditor(typeof(SceneBootstrapper))]
    public class SceneBootstrapperInspector : UnityEditor.Editor {

        private ReorderableList loadList;
        private string[] setNames;

        private void OnEnable() {
            // Create a reorderable list for manual scene input mode
            var sceneArray = serializedObject.FindProperty("scenesToLoad");
            PopulateList(ref loadList, serializedObject, sceneArray);

            // Prepare set names for manifest set picking mode
            var manifestProp = serializedObject.FindProperty("manifest");
            BuildSetNames(manifestProp.objectReferenceValue as SceneManifest);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            // Draw common settings
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneLoader"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoLoadScenes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadOptions"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadingScreenScene"));

            var loadProp = serializedObject.FindProperty("loadFromManifest");
            EditorGUILayout.PropertyField(loadProp);

            // If manifest loading is true, draw the properties needed to pick a set from the manifest
            if (loadProp.boolValue) {
                using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                    var manifestProp = serializedObject.FindProperty("manifest");
                    EditorGUILayout.PropertyField(manifestProp);

                    if (changeCheck.changed) {
                        BuildSetNames(manifestProp.objectReferenceValue as SceneManifest);
                    }
                }

                // Draw a popup to display set names
                var setProp = serializedObject.FindProperty("setName");
                var index = EditorGUILayout.Popup("Set Name", FindSetNameIndex(setProp.stringValue), setNames);
                if (index > -1) {
                    setProp.stringValue = setNames[index];
                }
            } else {  // Otherwise draw the manul controls
                loadList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void PopulateList(ref ReorderableList list, SerializedObject serializedObj, SerializedProperty sceneArray) {
            list = new ReorderableList(serializedObj, sceneArray);

            // Draw the list's header
            list.drawHeaderCallback = r => {
                EditorGUI.LabelField(r, "Scenes To Load");
                EditorGUI.LabelField(new Rect(r.x + r.width - 100f, r.y, 100f, r.height), "Is Main Scene?");
            };

            var height = EditorGUIUtility.singleLineHeight;
            var yOffset = (list.elementHeight - EditorGUIUtility.singleLineHeight) * 0.5f;

            // Draw the element in the list
            list.drawElementCallback = (r, i, active, focused) => {
                var element = sceneArray.GetArrayElementAtIndex(i);

                EditorGUI.PropertyField(new Rect(r.x, r.y + yOffset, r.width - 100f, height),
                    element.FindPropertyRelative("Name"), GUIContent.none);

                var mainSceneProp = element.FindPropertyRelative("IsMainScene");
                mainSceneProp.boolValue = EditorGUI.Toggle(
                    new Rect(r.x + r.width - 50f, r.y + yOffset, height, height),
                    mainSceneProp.boolValue);
            };
        }

        private void BuildSetNames(SceneManifest manifest) {
            if (manifest == null) {
                setNames = new string[0];
                return;
            }

            var sets = manifest.Sets;
            setNames = new string[sets.Length];

            for (var i = 0; i < setNames.Length; ++i) {
                setNames[i] = sets[i].Name;
            }
        }

        private int FindSetNameIndex(string name) {
            for (var i = 0; i < setNames.Length; ++i) {
                if (setNames[i] == name) {
                    return i;
                }
            }

            return -1;
        }
    }
}
