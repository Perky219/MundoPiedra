using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Config/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        [Header("Nombres exactos como están en Build Settings")]
        public string mainMenuSceneName = "MainMenu";
        public string gameSceneName = "SpawScene";
    }
}
