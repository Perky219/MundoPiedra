using System.Collections.Generic;
using UnityEngine;

public class BossSummoner : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject explosiveMeleePrefab;
    public GameObject explosiveRangedPrefab;

    [Header("Límites")]
    public int maxExplosivesPhase1 = 5;

    [Header("Cooldowns por fase")]
    public float phase3SummonCooldown = 7f;
    public float phase4SummonCooldown = 5f;

    BossPhaseController phaseController;

    List<GameObject> activeExplosives = new List<GameObject>();
    float nextSummonTime;

    void Awake()
    {
        phaseController = GetComponent<BossPhaseController>();
    }

    void Update()
    {
        CleanExplosiveList();

        if (!player || phaseController == null) return;

        switch (phaseController.CurrentPhase)
        {
            case BossPhase.Phase3:
                HandlePhase3();
                break;
            case BossPhase.Phase4:
                HandlePhase4();
                break;
        }
    }

    void HandlePhase3()
    {
        if (Time.time < nextSummonTime) return;

        // 50%–25%: invoca cerca y a distancia
        SummonFront();
        SummonFar();

        nextSummonTime = Time.time + phase3SummonCooldown;
    }

    void HandlePhase4()
    {
        if (Time.time < nextSummonTime) return;

        // <25%: invoca cerca o detrás del jugador
        SummonNearPlayerBehind();
        SummonNearPlayerBehind();

        nextSummonTime = Time.time + phase4SummonCooldown;
    }

    // ============= Métodos públicos/privados de invocación =============

    // Lo usará la fase 1 "cuando me pegan"
    public void TrySummonFrontPhase1()
    {
        if (CountActiveExplosives() >= maxExplosivesPhase1) return;
        SummonFront();
    }

    void SummonFront()
    {
        if (!explosiveMeleePrefab) return;

        Vector3 pos = transform.position + transform.forward * 2f;
        Quaternion rot = Quaternion.LookRotation(transform.forward);
        SpawnExplosive(explosiveMeleePrefab, pos, rot);
    }

    void SummonFar()
    {
        if (!explosiveRangedPrefab || !player) return;

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 pos = transform.position + dir * 8f;
        Quaternion rot = Quaternion.LookRotation(dir);

        SpawnExplosive(explosiveRangedPrefab, pos, rot);
    }

    void SummonNearPlayerBehind()
    {
        if (!explosiveMeleePrefab || !player) return;

        Vector3 behindDir = -player.forward;
        Vector3 pos = player.position + behindDir * 2f;
        Quaternion rot = Quaternion.LookRotation(-behindDir); // que mire al player

        SpawnExplosive(explosiveMeleePrefab, pos, rot);
    }

    void SpawnExplosive(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        GameObject go = Instantiate(prefab, pos, rot);
        activeExplosives.Add(go);
    }

    void CleanExplosiveList()
    {
        for (int i = activeExplosives.Count - 1; i >= 0; i--)
        {
            if (!activeExplosives[i])
                activeExplosives.RemoveAt(i);
        }
    }

    int CountActiveExplosives()
    {
        CleanExplosiveList();
        return activeExplosives.Count;
    }
}
