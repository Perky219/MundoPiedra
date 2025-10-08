using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI (opcional)")]
        [SerializeField] private GameObject btnResume; // arrastra BtnResume si existe en el prefab

        [Header("Config")]
        [SerializeField] private Game.Config.SceneConfig sceneConfig;

        private void Awake()
        {
            // En el menú inicial NO debe mostrarse "Reanudar"
            if (btnResume != null) btnResume.SetActive(false);

            Time.timeScale = 1f;
            AudioListener.pause = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void OnStartClicked()
        {
            string sceneName = sceneConfig != null ? sceneConfig.gameSceneName : "GameScene";

            if (!IsSceneInBuild(sceneName))
            {
                Debug.LogWarning($"[MainMenu] La escena '{sceneName}' no está en Build Settings. " + $"Deja el botón conectado; podrás probar cuando exista gameplay.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void OnQuitClicked()
        {
            Debug.Log("[MainMenu] Saliendo del juego...");
            Application.Quit();
        }

        private bool IsSceneInBuild(string name)
        {
            int count = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < count; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (sceneName == name) return true;
            }
            return false;
        }
    }
}