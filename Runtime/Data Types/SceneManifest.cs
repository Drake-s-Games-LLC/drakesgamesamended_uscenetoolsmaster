using UnityEngine;
using System.Collections;
using System;

namespace AdditiveSceneGroups {
    [Serializable]
    public struct SceneSet {
        public string Name;
        public SceneData[] Scenes;

        public static SceneSet Empty => new SceneSet { Name = string.Empty, Scenes = new SceneData[0] };
    }

    /// <summary>
    /// A collection of saved scene sets.  Values are editable in the editor
    /// </summary>
    [CreateAssetMenu(menuName = "AdditiveSceneGroups/Scene Manifest", fileName = "SceneManifest")]
    public class SceneManifest : ScriptableObject {

        public SceneSet[] Sets => sets;
        public int Length => sets.Length;

        public SceneSet this[string name] {
            get {
                for (var i = 0; i < sets.Length; ++i) {
                    if (sets[i].Name == name) {
                        return sets[i];
                    }
                }

                return SceneSet.Empty;
            }
        }

#pragma warning disable 649

        [SerializeField] private SceneSet[] sets = new SceneSet[0];

#pragma warning restore 659
    }
}
