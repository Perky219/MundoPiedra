using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DialogueProximityStarter : MonoBehaviour
{
    [Header("Yarn")]
    public DialogueRunner runner;         // arrastra aquí tu Dialogue Runner de la escena
    public string nodeName = "IntroCerbero"; // debe coincidir con 'title:' en tu .yarn

    [Header("Behaviour")]
    public bool oneShot = true;           // dispara solo una vez
    bool done;

    [Header("Scene Change")]
    public string nextSceneName = "Nivel1";

    void Reset()
    {
        // Asegura que sea trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // Si no hay rigidbody en este GO, sería buena idea tener uno cinemático en el padre/hijo
        // (pero no es estrictamente necesario si ya lo pusiste en el paso 5.3)
    }

    void OnEnable()
    {
        // Te suscribes al evento del DialogueRunner
        if (runner != null)
            runner.onDialogueComplete.AddListener(OnDialogueFinished);
    }
    
    void OnDisable()
    {
        // Te desuscribes del evento del DialogueRunner
        if (runner != null)
            runner.onDialogueComplete.RemoveListener(OnDialogueFinished);
    }

    void OnTriggerEnter(Collider other)
    {
        if (done && oneShot) return;               // ya se disparó una vez
        if (!other.CompareTag("Player")) return;   // solo el Player activa
        if (runner == null) return;
        if (runner.IsDialogueRunning) return;      // no interrumpas si ya hay diálogo

        runner.StartDialogue(string.IsNullOrEmpty(nodeName) ? null : nodeName);
        if (oneShot) done = true;
    }

    void OnDialogueFinished()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("[DialogueProximityStarter] No se configuró 'nextSceneName'.");
            return;
        }

        if (IsSceneInBuild(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning($"[DialogueProximityStarter] La escena '{nextSceneName}' no está en los ajustes de compilación.");
        }
    }
    
    private bool IsSceneInBuild(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}

