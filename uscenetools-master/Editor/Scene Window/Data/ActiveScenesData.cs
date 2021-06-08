namespace AdditiveSceneGroups.Editor {
    /// <summary>
    /// Contains a collection of scenes that are currently loaded into the editor
    /// </summary>
    public struct ActiveScenesData {
        public EditorSceneData[] Scenes;
        public int Hash;
    }
}
