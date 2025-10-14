using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class EnemigoArcher : MonoBehaviour
{
    // =========================
    // === PATRULLA ============
    // =========================
    [Header("Patrulla")]
    public int rutina;
    public float cronometro;
    public float velocidadGiroPatrulla = 120f;
    public float velocidadMovimiento = 2f;

    private Quaternion angulo;
    private float grado;

    // =========================
    // === ANIMACIÓN ===========
    // =========================
    [Header("Animación")]
    public Animator animator;
    public string triggerAttack = "attack";   // Trigger en tu Animator
    public string walkBool = "walk";          // Bool de caminar (si existe)

    private int walkHash = -1;
    private int attackHash = -1;

    [Header("Estado")]
    public bool atacando = false;
    private bool hasShotThisAttack = false;   // evita múltiples flechas

    // =========================
    // === DETECCIÓN / VISTA ===
    // =========================
    [Header("Detección del jugador")]
    public Transform jugador;
    public string tagJugador = "Player";
    public float distanciaDeteccion = 12f;

    [Header("Campo de visión (solo delante)")]
    [Range(0f, 360f)] public float fov = 100f;
    public float alturaOjos = 1.6f;

    [Header("Giro hacia el jugador")]
    public float velocidadGiroCuerpo = 240f;

    [Header("Raycast (línea de visión)")]
    public LayerMask mascaraRaycast = ~0;
    private float distActual = Mathf.Infinity;

    // =========================
    // ====== ATAQUE RANGED ====
    // =========================
    [Header("Ranged (flecha/proyectil)")]
    public GameObject proyectilPrefab;    // Weapon_Arrow (Rigidbody+Collider+ArrowProjectile)
    public Transform puntoDisparo;        // Empty en la cuerda (eje +Z hacia delante)
    public float velocidadProyectil = 25f;
    public float vidaProyectil = 5f;
    public int danioRanged = 5;

    [Header("Timing")]
    public float rangoAtaque = 8f;            // <= distanciaDeteccion
    public float tiempoEntreAtaques = 1.2f;   // cooldown
    private float proximoAtaque = 0f;
    private Coroutine ataqueCR;

    [Header("Ajuste de salida")]
    [Tooltip("Avanza un poco el spawn para no chocar con la cuerda/mano.")]
    public float offsetSalida = 0.03f;
    [Tooltip("Corrección angular si tu modelo tiene ejes raros (grados).")]
    public Vector3 offsetDisparoEuler = Vector3.zero;

    // =========================
    // ====== UNITY FLOW =======
    // =========================
    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        // Cache de parámetros (si existen)
        if (animator)
        {
            foreach (var p in animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Bool && p.name == walkBool) walkHash = Animator.StringToHash(walkBool);
                if (p.type == AnimatorControllerParameterType.Trigger && p.name == triggerAttack) attackHash = Animator.StringToHash(triggerAttack);
            }
        }

        // Autodescubrimiento defensivo del punto de disparo
        if (puntoDisparo == null)
        {
            puntoDisparo = transform.Find("ArrowSpawn")
                         ?? transform.Find("Bow/ArrowSpawn")
                         ?? transform.Find("Arco/ArrowSpawn");
        }
    }

    void Start()
    {
        if (jugador == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(tagJugador);
            if (go) jugador = go.transform;
        }

        if (jugador == null)
            Debug.LogWarning($"[Enemigo:{GetPath(transform)}] No se encontró el jugador. Asigna el Transform o usa tag '{tagJugador}'.");

        if (rangoAtaque > distanciaDeteccion) rangoAtaque = distanciaDeteccion;
    }

    void Update()
    {
        if (MirarJugadorSiTieneVista())
            return;

        Comportamiento_Enemigo();
    }

    // =========================
    // ====== PATRULLA =========
    // =========================
    void Comportamiento_Enemigo()
    {
        cronometro += Time.deltaTime;
        if (cronometro >= 2f)
        {
            rutina = Random.Range(0, 2); // 0: quieto, 1: elegir nuevo ángulo
            cronometro = 0f;
        }

        switch (rutina)
        {
            case 0:
                SetWalk(false);
                break;

            case 1:
                grado = Random.Range(0f, 360f);
                angulo = Quaternion.Euler(0f, grado, 0f);
                rutina = 2;
                break;

            case 2:
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, angulo, velocidadGiroPatrulla * Time.deltaTime
                );
                SetWalk(true);
                transform.Translate(Vector3.forward * velocidadMovimiento * Time.deltaTime, Space.Self);
                break;
        }
    }

    // =========================
    // === VISTA + ATAQUE ======
    // =========================
    bool MirarJugadorSiTieneVista()
    {
        if (jugador == null) return false;

        Vector3 ojo = transform.position + Vector3.up * alturaOjos;
        Vector3 cabezaJugador = AproximarCabeza(jugador);
        Vector3 toTarget = cabezaJugador - ojo;

        distActual = toTarget.magnitude;
        if (distActual > distanciaDeteccion) return false;

        // 1) FOV
        float ang = Vector3.Angle(transform.forward, toTarget);
        if (ang > fov * 0.5f) return false;

        // 2) Línea de visión
        if (!TieneLineaDeVision(ojo, toTarget.normalized, distActual))
            return false;

        // 3) Girar hacia el jugador
        Vector3 dirPlano = jugador.position - transform.position;
        dirPlano.y = 0f;
        if (dirPlano.sqrMagnitude > 0.0001f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(dirPlano);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, rotObjetivo, velocidadGiroCuerpo * Time.deltaTime
            );
        }

        SetWalk(false);

        // 4) Atacar
        if (distActual <= rangoAtaque)
            TryAtacar();

        return true;
    }

    void TryAtacar()
    {
        if (Time.time < proximoAtaque) return;
        if (atacando) return;

        atacando = true;
        hasShotThisAttack = false;            // reset antirrebote
        proximoAtaque = Time.time + tiempoEntreAtaques;

        if (animator != null && attackHash != -1)
            animator.SetTrigger(attackHash);
        else
        {
            EjecutarAtaque();
            FinDeAtaque();
        }

        if (ataqueCR != null) StopCoroutine(ataqueCR);
        ataqueCR = StartCoroutine(FinAtaquePorTiempo());
    }

    IEnumerator FinAtaquePorTiempo()
    {
        yield return new WaitForSeconds(Mathf.Max(0.05f, tiempoEntreAtaques));
        atacando = false;
        hasShotThisAttack = false;
        ataqueCR = null;
    }

    // Animation Event (momento del disparo)
    public void EjecutarAtaque()
    {
        if (hasShotThisAttack) return; // evita múltiples flechas por ataque
        hasShotThisAttack = true;
        DispararProyectil();
    }

    // Animation Event (fin del clip)
    public void FinDeAtaque()
    {
        atacando = false;
        hasShotThisAttack = false;
        if (ataqueCR != null) { StopCoroutine(ataqueCR); ataqueCR = null; }
    }

    // =========================
    // ===== DISPARO (FLECHA) ==
    // =========================
    void DispararProyectil()
    {
        if (proyectilPrefab == null || puntoDisparo == null)
        {
            Debug.LogWarning($"[Enemigo:{GetPath(transform)}] Falta " +
                $"{(proyectilPrefab==null?"proyectilPrefab ":"")}" +
                $"{(puntoDisparo==null?"puntoDisparo":"")}");
            return;
        }

        // 1) Posición de salida (ligeramente adelantada)
        Vector3 pos = puntoDisparo.position + puntoDisparo.forward * offsetSalida;

        // 2) Dirección base hacia el objetivo
        Vector3 dirObjetivo = (AproximarCabeza(jugador) - pos).normalized;

        // 3) Rotación final con micro-corrección
        Quaternion rot = Quaternion.LookRotation(dirObjetivo, Vector3.up) * Quaternion.Euler(offsetDisparoEuler);

        // 4) Instanciar
        GameObject go = Instantiate(proyectilPrefab, pos, rot);

        // 5) Dar velocidad
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false; // asegúrate que pueda moverse
            rb.velocity = (rot * Vector3.forward) * velocidadProyectil;
            rb.angularVelocity = Vector3.zero;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // 6) Pasar daño/owner si usas ArrowProjectile
        var arrow = go.GetComponent<ArrowProjectile>();
        if (arrow != null)
        {
            arrow.damage = danioRanged;
            arrow.owner  = this.transform; // para ignorar colisiones con el arquero
            // OnEnable del ArrowProjectile ya hace IgnoreCollision(owner)
        }

        if (vidaProyectil > 0f) Destroy(go, vidaProyectil);

        // Debug visual 1s
        Debug.DrawRay(pos, (rot * Vector3.forward) * 0.6f, Color.cyan, 1f);
    }

    // =========================
    // ====== HELPERS ==========
    // =========================
    bool TieneLineaDeVision(Vector3 origen, Vector3 dirNorm, float maxDist)
    {
        if (Physics.Raycast(origen, dirNorm, out RaycastHit hit, maxDist, mascaraRaycast, QueryTriggerInteraction.Ignore))
        {
            bool esJugador = hit.transform.CompareTag(tagJugador) || hit.transform.root.CompareTag(tagJugador);
            Debug.DrawLine(origen, hit.point, esJugador ? Color.green : Color.red, 0f, false);
            return esJugador;
        }
        Debug.DrawLine(origen, origen + dirNorm * maxDist, Color.yellow, 0f, false);
        return false;
    }

    Vector3 AproximarCabeza(Transform t) => t.position + Vector3.up * 1.6f;

    void SetWalk(bool value)
    {
        if (animator && walkHash != -1)
            animator.SetBool(walkHash, value);
    }

    string GetPath(Transform t)
    {
        return t == null ? "(null)" :
            (t.parent == null ? t.name : GetPath(t.parent) + "/" + t.name);
    }

    // =========================
    // ====== GIZMOS DEBUG =====
    // =========================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Vector3 origen = transform.position + Vector3.up * alturaOjos;
        Quaternion leftRot = Quaternion.AngleAxis(-fov * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(fov * 0.5f, Vector3.up);
        Vector3 leftDir = leftRot * transform.forward;
        Vector3 rightDir = rightRot * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origen, leftDir * distanciaDeteccion);
        Gizmos.DrawRay(origen, rightDir * distanciaDeteccion);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        if (puntoDisparo != null)
        {
            Vector3 pos = puntoDisparo.position + puntoDisparo.forward * offsetSalida;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos, 0.015f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(pos, puntoDisparo.forward * 0.6f);
        }
    }

    void OnValidate()
    {
        distanciaDeteccion = Mathf.Max(0f, distanciaDeteccion);
        rangoAtaque = Mathf.Clamp(rangoAtaque, 0f, distanciaDeteccion);
        fov = Mathf.Clamp(fov, 0f, 360f);
        tiempoEntreAtaques = Mathf.Max(0.05f, tiempoEntreAtaques);
        velocidadProyectil = Mathf.Max(0f, velocidadProyectil);
        offsetSalida = Mathf.Clamp(offsetSalida, 0f, 0.2f);
    }
}
