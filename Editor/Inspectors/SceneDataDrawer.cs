using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AdditiveSceneGroups.Editor {
    [CustomPropertyDrawer(typeof(SceneData))]
    public class SceneDataDrawer : PropertyDrawer {

        private Dictionary<string, SceneAsset> assets;
        private string[] mainSceneOptions = new[] { "---", "Active" };
        private bool isInitialized = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CheckInit();

#if UNITY_2019_2
            EditorGUI.PropertyField(position, property, label, true);
            return;
#else

            using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                var path = property.FindPropertyRelative("Path").stringValue;
                var selection = EditorGUI.ObjectField(
                    new Rect(position.x, position.y, position.width * 0.75f, position.height),
                    GUIContent.none,
                    FindAsset(path), typeof(SceneAsset), false);


                if (changeCheck.changed) {
                    property.FindPropertyRelative("Name").stringValue = selection.name;
                    property.FindPropertyRelative("Path").stringValue = AssetDatabase.GetAssetPath(selection);
                }
            }

            var mainSceneProp = property.FindPropertyRelative("IsMainScene");
            var active = EditorGUI.Popup(new Rect(position.x + position.width * 0.75f, position.y, position.width * 0.25f, position.height),
                string.Empty,
                mainSceneProp.boolValue ? 1 : 0,
                mainSceneOptions);

            mainSceneProp.boolValue = active > 0;

#endif
        }

#if UNITY_2019_2
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, true);
        }
#endif

        private void CheckInit() {
            if (!isInitialized) {
                assets = new Dictionary<string, SceneAsset>();

                var guids = AssetDatabase.FindAssets("t:SceneAsset");
                for (var i = 0; i < guids.Length; ++i) {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                    assets.Add(path, asset);
                }

                isInitialized = true;
            }
        }

        private SceneAsset FindAsset(string path) {
            if (assets.TryGetValue(path, out var asset)) {
                return asset;
            }

            return null;
        }
    }
}
