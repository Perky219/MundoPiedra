using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(BossPhaseController))]
[RequireComponent(typeof(BossSummoner))]
public class BossDamageWatcher : MonoBehaviour
{
    Health health;
    BossPhaseController phaseController;
    BossSummoner summoner;

    int lastHP;

    void Awake()
    {
        health = GetComponent<Health>();
        phaseController = GetComponent<BossPhaseController>();
        summoner = GetComponent<BossSummoner>();
    }

    void Start()
    {
        lastHP = health.currentHP;
    }

    void Update()
    {
        // Si por alguna razón no está inicializado, salimos
        if (health == null || phaseController == null || summoner == null) return;

        int current = health.currentHP;

        if (current < lastHP)
        {
            // Recibió daño
            if (phaseController.IsInPhase1())
            {
                // 100%–75%: cuando lo atacan, invoca explosivo delante (máx. 5)
                summoner.TrySummonFrontPhase1();
            }
        }

        // Actualizar el valor para el siguiente frame
        lastHP = current;
    }
}
