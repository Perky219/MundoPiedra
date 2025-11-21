using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class LavaCraterDamageUniversal : MonoBehaviour
{
    [Header("Config")]
    public int damage = 5;
    public string targetTag = "Player";
    [Tooltip("Si está activo, NO aplica daño real; solo marca dónde iría y hace logs.")]
    public bool simulateOnly = true; // Activa simulación por defecto para no romper nada

    // Evita múltiples golpes por múltiples colliders del mismo actor
    private HashSet<GameObject> inside = new HashSet<GameObject>();

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // Intentamos actuar sobre el root con Rigidbody si lo hay
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (inside.Contains(root)) return;
        inside.Add(root);

        if (simulateOnly)
        {
            Debug.Log($"[Lava] SIM MODE → aquí se aplicaría TakeDamage({damage}) a: {root.name}");
            return; // No hacemos nada más en simulación
        }

        // Sin dependencias: solo llamamos por nombre si existe. Si no, no revienta.
        root.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        inside.Remove(root);
    }

    // Dibuja el área del trigger en el editor para "señalar dónde va"
    void OnDrawGizmos()
    {
        var col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = simulateOnly ? new Color(1f, 0.5f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.35f);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider bc)
        {
            Gizmos.DrawCube(bc.center, bc.size);
            Gizmos.DrawWireCube(bc.center, bc.size);
        }
        else if (col is CapsuleCollider cc)
        {
            // Aproximación con caja para visualizar
            var size = new Vector3(cc.radius * 2f, cc.height, cc.radius * 2f);
            Gizmos.DrawCube(cc.center, size);
            Gizmos.DrawWireCube(cc.center, size);
        }
        else if (col is SphereCollider sc)
        {
            Gizmos.DrawSphere(sc.center, sc.radius);
        }
    }
}
