using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdditiveSceneGroups.Editor {
    public class SceneGroupButton : VisualElement {
        public bool IsExpanded { get { return isExpanded; } set { isExpanded = value; } }
        public int ButtonIndex { get; private set; }
        public string GroupName => groupProperty.FindPropertyRelative("Name").stringValue;

        private SerializedObject serializedBank;
        private SerializedProperty groupProperty;
        private Action onDelete;
        private Dictionary<string, SceneAsset> scenes;

        private Button groupButton;
        private IMGUIContainer editField;

        private bool isExpanded;
        private readonly string ExpandedClassName = "scenegroup-expanded";

        public SceneGroupButton(SerializedProperty property, SerializedObject serializedObject, int index, Dictionary<string, SceneAsset> scenes, Action onDelete) {
            groupProperty = property;
            serializedBank = serializedObject;
            ButtonIndex = index;
            this.onDelete = onDelete;
            this.scenes = scenes;

            // Create a row of buttons
            var buttonRow = new VisualElement { name = "button-row" };
            groupButton = new Button(Toggle) {
                text = property.FindPropertyRelative("Name").stringValue,
                name = "button-scenegroup"
            };
            buttonRow.Add(groupButton);
            buttonRow.Add(new Button(LoadScenes) { name = "button-load", text = "Load" });
            buttonRow.Add(new Button(DeleteElement) { name = "button-delete", text = "X" });

            editField = new IMGUIContainer(DrawEditField) { name = "imgui" };

            this.Add(buttonRow);
            this.Add(editField);
        }

        public void Expand() {
            isExpanded = true;

            this.AddToClassList(ExpandedClassName);
            groupButton.AddToClassList(ExpandedClassName);
            editField.AddToClassList(ExpandedClassName);
        }

        public void RefreshLabel() {
            groupButton.text = groupProperty.FindPropertyRelative("Name").stringValue;
        }

        public void Shrink() {
            isExpanded = false;

            this.RemoveFromClassList(ExpandedClassName);
            groupButton.RemoveFromClassList(ExpandedClassName);
            editField.RemoveFromClassList(ExpandedClassName);
        }

        public void Toggle() {
            isExpanded = !isExpanded;

            if (isExpanded) {
                this.AddToClassList(ExpandedClassName);
                groupButton.AddToClassList(ExpandedClassName);
                editField.AddToClassList(ExpandedClassName);
            } else {
                this.RemoveFromClassList(ExpandedClassName);
                groupButton.RemoveFromClassList(ExpandedClassName);
                editField.RemoveFromClassList(ExpandedClassName);
            }
        }

        private void DrawEditField() {
            if (isExpanded && groupProperty != null) {
                serializedBank.Update();

                var box = EditorGUILayout.BeginVertical();

                using (var changeCheck = new EditorGUI.ChangeCheckScope()) {
                    EditorGUILayout.PropertyField(groupProperty.FindPropertyRelative("Name"), false);
                    EditorGUILayout.PropertyField(groupProperty.FindPropertyRelative("Scenes"), true);

                    if (changeCheck.changed) {
                        serializedBank.ApplyModifiedProperties();
                        groupButton.text = groupProperty.FindPropertyRelative("Name").stringValue;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void LoadScenes() {
            var manifest = serializedBank.targetObject as SceneManifest;
            EditorSceneUtility.LoadSceneGroup(manifest.Sets[ButtonIndex]);
        }

        private void DeleteElement() {
            serializedBank.Update();
            var setArr = serializedBank.FindProperty("sets");

            setArr.DeleteArrayElementAtIndex(ButtonIndex);
            serializedBank.ApplyModifiedProperties();

            onDelete?.Invoke();
        }
    }
}
