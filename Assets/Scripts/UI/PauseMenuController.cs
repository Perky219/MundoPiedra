using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject pauseMenuUI; // raíz del overlay de pausa (panel/card)
        [Header("Config")]
        [SerializeField] private Game.Config.SceneConfig sceneConfig;

        private bool isPaused;

        private void Start()
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            isPaused = false;

            Time.timeScale = 1f;
            AudioListener.pause = false;
            HideCursorDuringPlay();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) Resume();
                else Pause();
            }
        }

        public void Resume()
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
            isPaused = false;
            HideCursorDuringPlay();
        }

        public void Pause()
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
            isPaused = true;
            ShowCursor();
        }

        public void QuitToMainMenu()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;

            string menuName = sceneConfig != null ? sceneConfig.mainMenuSceneName : "MainMenu";
            if (!IsSceneInBuild(menuName))
            {
                Debug.LogWarning($"[PauseMenu] La escena '{menuName}' no está en Build Settings.");
                return;
            }

            SceneManager.LoadScene(menuName);
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HideCursorDuringPlay()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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