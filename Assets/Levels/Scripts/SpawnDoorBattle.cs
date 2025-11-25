using UnityEngine;
using System.Collections;

public class SpawnDoorBattle : MonoBehaviour
{
    [Header("Prefabs y posiciones")]
    [SerializeField] private GameObject doorPrefab;          // Prefab de la puerta
    [SerializeField] private Transform doorSpawnPoint;       // Punto donde aparece la puerta

    [Header("Boss")]
    [SerializeField] private GameObject bossObject;          // Jefe ya existente en la escena (desactivado al inicio)

    [Header("Apertura de puerta")]
    [SerializeField] private Vector3 openMoveOffset = new Vector3(0, 2f, 0);
    [SerializeField] private float moveSpeed = 2f;

    [Header("Comportamiento")]
    [SerializeField] private bool oneShot = true;

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

        triggered = true;
        StartCoroutine(RoomSequenceRealtime());
    }

    private IEnumerator RoomSequenceRealtime()
    {
        Debug.Log(">>> [DEBUG] Entrando a RoomSequenceRealtime");

        // Info de referencias
        Debug.Log(">>> [DEBUG] doorPrefab = " + doorPrefab);
        Debug.Log(">>> [DEBUG] doorSpawnPoint = " + doorSpawnPoint);
        Debug.Log(">>> [DEBUG] bossObject = " + bossObject);

        // 1. Instanciar puerta
        spawnedDoor = Instantiate(doorPrefab, doorSpawnPoint.position, doorSpawnPoint.rotation);
        Debug.Log(">>> [DEBUG] Puerta instanciada: " + spawnedDoor);
        Debug.Log(">>> [DEBUG] Posición inicial puerta: " + spawnedDoor.transform.position);

        // 2. Activar boss
        if (bossObject != null)
        {
            Debug.Log(">>> [DEBUG] Activando boss...");
            bossObject.SetActive(true);
            Debug.Log(">>> [DEBUG] bossObject.activeSelf = " + bossObject.activeSelf);
        }
        else
        {
            Debug.LogWarning(">>> [DEBUG] bossObject es NULL. No hay combate.");
        }

        // 3. Esperar hasta que el boss muera/desaparezca
        Debug.Log(">>> [DEBUG] Esperando muerte del boss...");

        int loops = 0;
        while (bossObject != null && bossObject.activeInHierarchy)
        {
            loops++;
            if (loops % 60 == 0)
                Debug.Log(">>> [DEBUG] Boss sigue vivo...");
            yield return null;
        }

        Debug.Log(">>> [DEBUG] Boss derrotado.");

        // 4. Mover y destruir puerta
        if (spawnedDoor != null)
        {
            Debug.Log(">>> [DEBUG] Abriendo puerta...");
            Vector3 target = spawnedDoor.transform.position + openMoveOffset;
            yield return MoveDoorRealtime(spawnedDoor, target);

            Debug.Log(">>> [DEBUG] Destruyendo puerta...");
            Destroy(spawnedDoor);
        }
        else
        {
            Debug.LogWarning(">>> [DEBUG] No hay puerta para abrir/destruir");
        }

        Debug.Log(">>> [DEBUG] Secuencia finalizada.");

        if (!oneShot) triggered = false;
    }

    private IEnumerator MoveDoorRealtime(GameObject door, Vector3 target)
    {
        Debug.Log(">>> [DEBUG] Moviendo puerta hacia " + target);

        while (door != null && Vector3.Distance(door.transform.position, target) > 0.01f)
        {
            float step = moveSpeed * Time.unscaledDeltaTime;
            door.transform.position = Vector3.MoveTowards(door.transform.position, target, step);
            yield return null;
        }

        Debug.Log(">>> [DEBUG] Puerta alcanzó destino.");
    }

    // Dibujar el punto de spawn en la escena
    private void OnDrawGizmos()
    {
        if (doorSpawnPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(doorSpawnPoint.position, 0.2f);
        Gizmos.DrawWireCube(doorSpawnPoint.position, new Vector3(1, 2, 0.1f));
    }
}
