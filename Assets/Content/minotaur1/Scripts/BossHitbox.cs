using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    [Header("Setup")]
    public BossAI ownerAI;
    public int damage = 4;
    public string targetTag = "Player";
    public float perTargetCooldown = 0.7f;

    Dictionary<Health, float> lastHitTime = new();

    void Awake()
    {
        if (!ownerAI) ownerAI = GetComponentInParent<BossAI>();
    }

    void OnTriggerStay(Collider other)
    {
        if (ownerAI == null || !ownerAI.IsAttackActive) return;
        if (!other.CompareTag(targetTag)) return;

        Health hp = other.GetComponent<Health>();
        if (hp == null) return;

        float t = Time.time;

        // evitar daño doble en la misma animación
        if (ownerAI.hasDealtDamageThisSwing) return;

        // cooldown por objetivo
        if (lastHitTime.TryGetValue(hp, out float last) && (t - last) < perTargetCooldown)
            return;

        // rango físico seguro
        float maxDist = ownerAI.attackRange + 0.4f;
        float dist = Vector3.Distance(ownerAI.transform.position, other.transform.position);
        if (dist > maxDist) return;

        // Aplicar daño
        hp.TakeDamage(damage);
        ownerAI.hasDealtDamageThisSwing = true;
        lastHitTime[hp] = t;
    }

    void OnTriggerExit(Collider other)
    {
        Health hp = other.GetComponent<Health>();
        if (hp != null) lastHitTime.Remove(hp);
    }
}
