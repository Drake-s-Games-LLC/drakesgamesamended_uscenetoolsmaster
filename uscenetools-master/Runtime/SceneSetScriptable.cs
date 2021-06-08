using System;
using UnityEngine;

namespace AdditiveSceneGroups
{
    [Serializable]
    [CreateAssetMenu(menuName = "AdditiveSceneGroups/Scene Set", fileName = "Scene Set")]
    public class SceneSetScriptable : ScriptableObject
    {
        [SerializeField] public SceneSet SceneSet;
        [SerializeField] public LoadOptions loadOption;
    }
}
