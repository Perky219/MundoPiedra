using System.Collections.Generic;
using UnityEngine;

public class SlowBossHitbox : MonoBehaviour
{
    [Header("Setup")]
    public SlowBossAI ownerAI;           // referencia al boss
    public int damage = 4;               // daño por golpe
    public string targetTag = "Player";  // tag del objetivo
    public float perTargetCooldown = 0.7f; // tiempo entre golpes por objetivo

    Dictionary<Health, float> lastHitTime = new();

    void Awake()
    {
        if (!ownerAI) ownerAI = GetComponentInParent<SlowBossAI>();
    }

    void OnTriggerStay(Collider other)
    {
        if (ownerAI == null || !ownerAI.IsAttackActive) return;
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        var hp = other.GetComponent<Health>();
        if (hp == null) return;

        float t = Time.time;
        if (lastHitTime.TryGetValue(hp, out float last) && (t - last) < perTargetCooldown) return;

        float maxDist = ownerAI.attackRange + 0.4f;
        float dist = Vector3.Distance(ownerAI.transform.position, other.transform.position);
        if (dist > maxDist) return;

        hp.TakeDamage(damage);
        lastHitTime[hp] = t;
    }

    void OnTriggerExit(Collider other)
    {
        var hp = other.GetComponent<Health>();
        if (hp != null) lastHitTime.Remove(hp);
    }
}
