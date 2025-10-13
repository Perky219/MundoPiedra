using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BigSpikes : MonoBehaviour
{
    [Header("Daño por segundo")]
    [Tooltip("Cuánto daño aplica por segundo mientras el objetivo esté tocando los pinchos.")]
    public float damagePerSecond = 10f;

    [Header("Filtrado")]
    [Tooltip("Solo dañará objetos con este tag (ej: Player).")]
    public string targetTag = "Player";

    // Acumulador por objetivo para convertir DPS (float) a golpes enteros para Health.TakeDamage(int)
    private readonly Dictionary<Health, float> _accumulators = new Dictionary<Health, float>();

    private Collider _col;

    void Awake()
    {
        _col = GetComponent<Collider>();
        // Sugerencia: si quieres daño por volumen, marca el collider como isTrigger
        // si quieres daño por contacto físico, déjalo sin isTrigger.
    }

    void Update()
    {
        if (_accumulators.Count == 0) return;

        float add = damagePerSecond * Time.deltaTime;

        // Recolectamos a quiénes aplicar daño esta frame
        // y mantenemos acumulador parcial para resto decimal
        var toApply = new List<(Health hp, int dmg)>();

        foreach (var kvp in _accumulators)
        {
            var hp = kvp.Key;
            float acc = kvp.Value + add;

            int whole = Mathf.FloorToInt(acc);
            if (whole > 0)
            {
                toApply.Add((hp, whole));
                _accumulators[hp] = acc - whole; // conserva la fracción
            }
            else
            {
                _accumulators[hp] = acc;
            }
        }

        // Aplicamos daño acumulado
        foreach (var item in toApply)
        {
            if (item.hp != null)
                item.hp.TakeDamage(item.dmg);
        }
    }

    // ---- Trigger path ----
    private void OnTriggerEnter(Collider other)
    {
        if (!_col.isTrigger) return;
        TryAddTarget(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_col.isTrigger) return;
        // Nada que hacer aquí; el daño se calcula en Update con el acumulador
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_col.isTrigger) return;
        TryRemoveTarget(other);
    }

    // ---- Collision path ----
    private void OnCollisionEnter(Collision collision)
    {
        if (_col.isTrigger) return;
        TryAddTarget(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_col.isTrigger) return;
        // Daño en Update
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_col.isTrigger) return;
        TryRemoveTarget(collision.collider);
    }

    // ---- Helpers ----
    private void TryAddTarget(Collider col)
    {
        if (!col || !col.CompareTag(targetTag)) return;

        Health hp = col.GetComponent<Health>();
        if (hp != null && !_accumulators.ContainsKey(hp))
        {
            _accumulators.Add(hp, 0f);
        }
    }

    private void TryRemoveTarget(Collider col)
    {
        if (!col) return;

        Health hp = col.GetComponent<Health>();
        if (hp != null && _accumulators.ContainsKey(hp))
        {
            _accumulators.Remove(hp);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        var c = GetComponent<Collider>();
        if (c is BoxCollider b)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(b.center, b.size);
        }
    }
#endif
}
