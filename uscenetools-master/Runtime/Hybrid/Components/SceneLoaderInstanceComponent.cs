using System;
using Unity.Entities;

namespace AdditiveSceneGroups.ECS {

    [Serializable]
    public struct SceneLoaderInstance : ISharedComponentData, IEquatable<SceneLoaderInstance> {
        public SceneLoader Value;

        public bool Equals(SceneLoaderInstance other) => other.Value == Value;
        public override int GetHashCode() {
            var hash = base.GetHashCode();

            if (!ReferenceEquals(null, Value)) hash ^= Value.GetHashCode();

            return hash;
        }
    }

    public class SceneLoaderInstanceComponent : SharedComponentDataProxy<SceneLoaderInstance> { }
}
