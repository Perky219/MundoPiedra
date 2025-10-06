using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(Collider))]
public class DialogueProximityStarter : MonoBehaviour
{
    [Header("Yarn")]
    public DialogueRunner runner;         // arrastra aquí tu Dialogue Runner de la escena
    public string nodeName = "IntroCerbero"; // debe coincidir con 'title:' en tu .yarn

    [Header("Behaviour")]
    public bool oneShot = true;           // dispara solo una vez
    bool done;

    void Reset() {
        // Asegura que sea trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // Si no hay rigidbody en este GO, sería buena idea tener uno cinemático en el padre/hijo
        // (pero no es estrictamente necesario si ya lo pusiste en el paso 5.3)
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
}

