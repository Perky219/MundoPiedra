using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ArrowProjectile : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 5;
    public LayerMask hitMask = ~0;               // capas válidas para impactar

    [Header("Comportamiento al impactar")]
    public bool stickOnHit = true;               // se clava
    public float lifeAfterHit = 8f;              // tiempo hasta destruirse tras impactar

    [Header("Dueño (ignorar colisiones)")]
    public Transform owner;                      // quien disparó
    public float ownerIgnoreTime = 0.25f;        // cuánto ignorar al dueño tras salir

    [Header("Vuelo")]
    public bool alignToVelocity = true;          // alinear punta con dirección de vuelo
    public float minSpeedToAlign = 0.1f;         // umbral para alinear

    Rigidbody rb;
    Collider col;
    bool hasHit = false;
    bool ignoringOwner = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Asegura configuración de físicas razonable
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Usaremos colisiones físicas (no triggers) por defecto
        col.isTrigger = false;
    }

    void OnEnable()
    {
        // Reset pool-friendly
        hasHit = false;
        if (rb)
        {
            rb.isKinematic = false;          // importante: libre para moverse
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.detectCollisions = true;
        }
        if (col) col.enabled = true;

        // Ignorar colliders del dueño un rato
        if (owner != null)
        {
            IgnoreOwnerCollisions(true);
            if (ownerIgnoreTime > 0f)
                Invoke(nameof(StopIgnoringOwner), ownerIgnoreTime);
        }
    }

    void OnDisable()
    {
        // Limpieza por si se usa pooling
        CancelInvoke(nameof(StopIgnoringOwner));
        if (ignoringOwner) IgnoreOwnerCollisions(false);
        ignoringOwner = false;
        hasHit = false;
    }

    void Update()
    {
        // Alinear la punta con la velocidad en vuelo (queda más realista)
        if (!hasHit && alignToVelocity && rb != null)
        {
            Vector3 v = rb.velocity;
            if (v.sqrMagnitude > (minSpeedToAlign * minSpeedToAlign))
            {
                transform.rotation = Quaternion.LookRotation(v.normalized, Vector3.up);
            }
        }
    }

    void StopIgnoringOwner()
    {
        IgnoreOwnerCollisions(false);
        ignoringOwner = false;
    }

    void IgnoreOwnerCollisions(bool ignore)
    {
        if (owner == null || col == null) return;

        var ownerCols = owner.GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (var oc in ownerCols)
        {
            if (oc) Physics.IgnoreCollision(col, oc, ignore);
        }
        ignoringOwner = ignore;
    }

    // ----- IMPACTO POR COLISIÓN FÍSICA -----
    void OnCollisionEnter(Collision c)
    {
        if (hasHit) return;

        if (!LayerIsInMask(c.gameObject.layer, hitMask)) return;

        // Seguridad extra: sigue ignorando al dueño si aún lo toca
        if (owner != null && (c.collider.transform == owner || c.collider.transform.IsChildOf(owner)))
            return;

        DoHit(c.collider, c.contacts.Length > 0 ? (Vector3?)c.contacts[0].point : null, c.contacts.Length > 0 ? (Vector3?)-c.contacts[0].normal : null);
    }

    // ----- IMPACTO POR TRIGGER (si algún objetivo usa isTrigger=true) -----
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (!LayerIsInMask(other.gameObject.layer, hitMask)) return;

        if (owner != null && (other.transform == owner || other.transform.IsChildOf(owner)))
            return;

        // Como no hay ContactPoint, aproximamos en el centro del collider
        DoHit(other, transform.position, null);
    }

    void DoHit(Collider hitCol, Vector3? contactPoint, Vector3? hitNormal)
    {
        hasHit = true;

        // 1) Aplicar daño si existe Health
        var hp = hitCol.GetComponentInParent<Health>();
        if (hp != null) hp.TakeDamage(damage);

        // 2) Clavarse (orden correcto para evitar error de kinematic+velocity)
        if (stickOnHit && rb != null)
        {
            rb.velocity = Vector3.zero;               // primero parar
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;                    // luego kinematic
            rb.detectCollisions = false;

            if (col) col.enabled = false;

            // Pegar y alinear
            Transform parent = hitCol.transform;
            if (contactPoint.HasValue) transform.position = contactPoint.Value;

            if (hitNormal.HasValue)
                transform.rotation = Quaternion.LookRotation(hitNormal.Value, Vector3.up);
            else if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);

            transform.SetParent(parent, true);
        }

        // 3) Auto-destrucción
        if (lifeAfterHit > 0f) Destroy(gameObject, lifeAfterHit);
    }

    bool LayerIsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
