using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SpawnDoorBattle : MonoBehaviour
{
    [Header("Prefabs y posiciones")]
    [SerializeField] private GameObject doorPrefab;      
    [SerializeField] private Transform doorSpawnPoint;   

    [Header("Boss")]
    [SerializeField] private GameObject bossObject;      

    [Header("Apertura de puerta")]
    [SerializeField] private Vector3 openMoveOffset = new Vector3(0, 2f, 0);
    [SerializeField] private float moveSpeed = 2f;

    [Header("Comportamiento")]
    [SerializeField] private bool oneShot = true;

    [Header("Transición de escena")]
    [SerializeField] private string nextSceneName = "WinScene";  // <- poné aquí el nombre exacto de la escena
    [SerializeField] private float delayBeforeSceneLoad = 2f; // segundos antes de cambiar

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

        if (doorPrefab == null || doorSpawnPoint == null)
            return;

        triggered = true;
        StartCoroutine(RoomSequenceRealtime());
    }

    private IEnumerator RoomSequenceRealtime()
    {
        // 1. Instanciar puerta
        spawnedDoor = Instantiate(doorPrefab, doorSpawnPoint.position, doorSpawnPoint.rotation);

        // 2. Activar boss
        if (bossObject != null)
            bossObject.SetActive(true);

        // 3. Esperar hasta que el boss muera
        while (bossObject != null && bossObject.activeInHierarchy)
            yield return null;

        // 4. Mover y destruir puerta
        if (spawnedDoor != null)
        {
            Vector3 target = spawnedDoor.transform.position + openMoveOffset;
            yield return MoveDoorRealtime(spawnedDoor, target);
            Destroy(spawnedDoor);
        }

        // 5. Espera antes de cargar el siguiente nivel
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // 6. Cargar siguiente nivel
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);

        if (!oneShot) triggered = false;
    }

    private IEnumerator MoveDoorRealtime(GameObject door, Vector3 target)
    {
        while (door != null && Vector3.Distance(door.transform.position, target) > 0.01f)
        {
            float step = moveSpeed * Time.unscaledDeltaTime;
            door.transform.position = Vector3.MoveTowards(door.transform.position, target, step);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (doorSpawnPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(doorSpawnPoint.position, 0.2f);
        Gizmos.DrawWireCube(doorSpawnPoint.position, new Vector3(1, 2, 0.1f));
    }
}
