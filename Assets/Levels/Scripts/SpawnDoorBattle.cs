using UnityEngine;
using System.Collections;

public class SpawnDoorBattle : MonoBehaviour
{
    [Header("Prefabs y posiciones")]
    [SerializeField] private GameObject doorPrefab;      // Prefab de la puerta que se instanciará
    [SerializeField] private Transform doorSpawnPoint;   // Punto donde aparecerá la puerta

    [Header("Simulación de combate")]
    [SerializeField] private float battleDurationSeconds = 8f;  // Duración del “combate”
    [SerializeField] private Vector3 openMoveOffset = new Vector3(0, 2f, 0); // Movimiento al abrirse
    [SerializeField] private float moveSpeed = 2f;

    [Header("Comportamiento")]
    [SerializeField] private bool oneShot = true;  // Si está activado, solo se ejecuta una vez

    [Header("Debug")]
    [SerializeField] private bool logVerbose = true;

    private GameObject spawnedDoor;
    private bool triggered;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (!other.CompareTag("Player")) return;

        // Validaciones básicas
        if (doorPrefab == null)
        {
            Debug.LogError("[SpawnDoorBattle] doorPrefab no asignado.");
            return;
        }
        if (doorSpawnPoint == null)
        {
            Debug.LogError("[SpawnDoorBattle] doorSpawnPoint no asignado.");
            return;
        }

        // Aviso sobre Rigidbody (necesario para triggers)
        if (other.attachedRigidbody == null && GetComponent<Rigidbody>() == null)
        {
            Debug.LogWarning("[SpawnDoorBattle] Ni el Player ni el Trigger tienen Rigidbody. " +
                             "OnTriggerEnter puede no disparar en algunos setups. " +
                             "Pon Rigidbody (Kinematic) en el Player o en este trigger.");
        }

        triggered = true;
        StartCoroutine(RoomSequenceRealtime());
    }

    private IEnumerator RoomSequenceRealtime()
    {
        // 1) Spawnea la puerta
        spawnedDoor = Instantiate(doorPrefab, doorSpawnPoint.position, doorSpawnPoint.rotation);
        if (logVerbose) Debug.Log("[SpawnDoorBattle] Puerta instanciada en " + doorSpawnPoint.position);

        // 2) Simula la batalla (tiempo real)
        float t0 = Time.realtimeSinceStartup;
        float tEnd = t0 + Mathf.Max(0f, battleDurationSeconds);
        if (logVerbose) Debug.Log("[SpawnDoorBattle] Simulando batalla por " + battleDurationSeconds + "s (realtime).");

        while (Time.realtimeSinceStartup < tEnd)
            yield return null;

        // 3) Mueve o destruye la puerta al terminar
        if (spawnedDoor != null)
        {
            Vector3 target = spawnedDoor.transform.position + openMoveOffset;
            yield return MoveDoorRealtime(spawnedDoor, target);

            if (logVerbose) Debug.Log("[SpawnDoorBattle] Batalla terminada. Puerta abierta y destruida.");
            Destroy(spawnedDoor);
        }

        if (!oneShot) triggered = false;  // Si no es “solo una vez”, permite reactivarlo
    }

    private IEnumerator MoveDoorRealtime(GameObject door, Vector3 target)
    {
        // Movimiento independiente de Time.timeScale
        while (door != null && Vector3.Distance(door.transform.position, target) > 0.01f)
        {
            float step = moveSpeed * Time.unscaledDeltaTime;
            door.transform.position = Vector3.MoveTowards(door.transform.position, target, step);
            yield return null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (doorSpawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(doorSpawnPoint.position, Vector3.one * 0.5f);
        }
    }
#endif
}