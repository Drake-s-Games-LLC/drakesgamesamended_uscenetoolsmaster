using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// A Widget that displays a collection of saved groups
    /// </summary>
    public class SceneGroupListWidget : VisualElement {

        private ScrollView scrollView;
        private SceneGroupButton[] buttons;

        private IEditorSceneManager sceneManager;


        public SceneGroupListWidget(IEditorSceneManager sceneManager) {
            this.sceneManager = sceneManager;
            scrollView = new ScrollView(ScrollViewMode.Vertical);
            buttons = new SceneGroupButton[0];

            // Add a title bar
            var titleBar = new VisualElement { name = "group-list-title" };
            titleBar.Add(new Label("Saved Groups") { name = "inspector-heading" });
            titleBar.Add(new Button(SortButtons) { name = "button-sort", text = "Sort" });

            this.Add(titleBar);
            this.Add(scrollView);

            RefreshGroupList(sceneManager.Manifest);
            sceneManager.SceneManifestChanged += RefreshGroupList;
        }

        private Dictionary<string, SceneAsset> FetchSceneAssets() {
            var guids = AssetDatabase.FindAssets("t:SceneAsset");
            var assets = new Dictionary<string, SceneAsset>();

            for (var i = 0; i < guids.Length; ++i) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                assets.Add(path, asset);
            }

            return assets;
        }

        private void RefreshGroupList(SceneManifest manifest) {
            if (manifest.Length != buttons.Length) {
                RemoveExistingButtons(buttons);

                var container = scrollView.contentContainer;
                var serializedBank = sceneManager.SerializedManifest;
                serializedBank.Update();
                var array = sceneManager.SerializedSets;
                buttons = new SceneGroupButton[array.arraySize];

                var projectScenes = FetchSceneAssets();

                for (var i = 0; i < buttons.Length; ++i) {
                    var element = array.GetArrayElementAtIndex(i);

                    var button = new SceneGroupButton(element, serializedBank, i, projectScenes, sceneManager.NotifyManifestUpdate);
                    container.Add(button);
                    buttons[i] = button;
                }
            } else {
                sceneManager.SerializedManifest.Update();

                for (var i = 0; i < buttons.Length; ++i) {
                    buttons[i].RefreshLabel();
                }
            }
        }

        private void RemoveExistingButtons(SceneGroupButton[] buttons) {
            var container = scrollView.contentContainer;

            for (var i = 0; i < buttons.Length; ++i) {
                container.Remove(buttons[i]);
            }
        }

        private void SortButtons() {
            // Release the buttons
            var container = scrollView.contentContainer;
            while (container.childCount > 0) {
                container.RemoveAt(0);
            }

            Array.Sort(buttons, (a, b) => { return a.GroupName.CompareTo(b.GroupName); });

            // Readd the buttons to the list
            for (var i = 0; i < this.buttons.Length; ++i) {
                container.Add(buttons[i]);
            }
        }
    }
}
