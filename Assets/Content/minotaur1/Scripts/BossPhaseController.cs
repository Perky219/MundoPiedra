using UnityEngine;

public enum BossPhase
{
    Phase1, // 100% - 75%
    Phase2, // 75% - 50%
    Phase3, // 50% - 25%
    Phase4  // < 25%
}

[RequireComponent(typeof(Health))]
public class BossPhaseController : MonoBehaviour
{
    public BossPhase CurrentPhase { get; private set; }

    Health health;

    [Header("Umbrales de fases (porcentaje)")]
    [Range(0f, 1f)] public float phase2Threshold = 0.75f; // <75% → fase 2
    [Range(0f, 1f)] public float phase3Threshold = 0.50f; // <50% → fase 3
    [Range(0f, 1f)] public float phase4Threshold = 0.25f; // <25% → fase 4

    void Awake()
    {
        health = GetComponent<Health>();
        UpdatePhase();
    }

    void Update()
    {
        UpdatePhase();
    }

    void UpdatePhase()
    {
        if (health.maxHP <= 0) return;

        float pct = (float)health.currentHP / health.maxHP;

        if (pct > phase2Threshold)
        {
            CurrentPhase = BossPhase.Phase1;
        }
        else if (pct > phase3Threshold)
        {
            CurrentPhase = BossPhase.Phase2;
        }
        else if (pct > phase4Threshold)
        {
            CurrentPhase = BossPhase.Phase3;
        }
        else
        {
            CurrentPhase = BossPhase.Phase4;
        }
    }

    // Helpers cómodos
    public bool CanRun()
    {
        // A partir de fase 2 corre
        return CurrentPhase == BossPhase.Phase2
            || CurrentPhase == BossPhase.Phase3
            || CurrentPhase == BossPhase.Phase4;
    }

    public bool IsInPhase1()
    {
        return CurrentPhase == BossPhase.Phase1;
    }
}
