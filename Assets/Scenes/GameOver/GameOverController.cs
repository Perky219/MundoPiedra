using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Tooltip("Segundos que se queda la pantalla de Game Over antes de ir al menú")]
    public float delayToMenu = 3f;

    void Start()
    {
        // Asegurar tiempo normal
        Time.timeScale = 1f;

        StartCoroutine(GoToMenuAfterDelay());
    }

    System.Collections.IEnumerator GoToMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayToMenu);
        SceneManager.LoadScene("MainMenu");
    }
}
