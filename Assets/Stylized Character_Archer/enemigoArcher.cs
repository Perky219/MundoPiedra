using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoArcher : MonoBehaviour
{
    // --- Tu lógica existente ---
    public int rutina;
    public float cronometro;
    public Animator animator;
    public Quaternion angulo;
    public float grado;

    [Header("Estado ataque")]
    public bool atacando = false;

    // --- Detección / Mirada (sin IK) ---
    [Header("Detección del jugador")]
    public Transform jugador;                  // Asignar en Inspector o por tag
    public string tagJugador = "Player";
    public float distanciaDeteccion = 12f;     // radio de detección (m)

    [Header("Campo de visión (solo delante)")]
    [Range(0f, 360f)] public float fov = 100f; // ancho del cono de visión (grados)
    public float alturaOjos = 1.6f;            // punto desde donde “mira” el enemigo

    [Header("Giros y movimiento")]
    public float velocidadGiroCuerpo = 240f;   // °/s al mirar al jugador
    public float velocidadGiroPatrulla = 120f; // °/s al girar en patrulla
    public float velocidadMovimiento = 2f;     // u/s en patrulla

    // --- Ataque ---
    public enum TipoAtaque { Ranged, Melee }
    [Header("Ataque")]
    public TipoAtaque tipoAtaque = TipoAtaque.Ranged;
    public float rangoAtaque = 8f;             // debe ser <= distanciaDeteccion
    public float tiempoEntreAtaques = 1.2f;    // cooldown
    float proximoAtaque = 0f;

    [Header("Ranged (proyectil)")]
    public GameObject proyectilPrefab;         // flecha/bala con Rigidbody + collider
    public Transform puntoDisparo;             // empty en la mano/arco
    public float velocidadProyectil = 25f;
    public float vidaProyectil = 5f;

    [Header("Melee")]
    public int danioMelee = 10;
    public float radioMelee = 1.5f;
    public LayerMask mascaraMelee = ~0;        // incluye capa del Player

    // Raycast LOS
    [Header("Raycast (línea de visión)")]
    public LayerMask mascaraRaycast = ~0;      // muros + jugador
    float distActual = Mathf.Infinity;         // distancia al jugador (para debug/ataque)

    void Start()
    {
        animator = GetComponent<Animator>();
        if (jugador == null)
        {
            var go = GameObject.FindGameObjectWithTag(tagJugador);
            if (go != null) jugador = go.transform;
        }
        if (jugador == null)
            Debug.LogWarning("[Enemigo] No se encontró el jugador. Asigna el Transform o usa tag 'Player'.");
    }

    void Update()
    {
        // Si el jugador está en vista (en rango, dentro del FOV y sin muro), mirarlo y atacar
        if (MirarJugadorSiTieneVista())
            return;

        // Si no, patrullar
        Comportamiento_Enemigo();
    }

    // --- Patrulla ---
    public void Comportamiento_Enemigo()
    {
        cronometro += Time.deltaTime;
        if (cronometro >= 2f)
        {
            rutina = Random.Range(0, 2); // 0 o 1
            cronometro = 0f;
        }

        switch (rutina)
        {
            case 0:
                animator.SetBool("walk", false);
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
                animator.SetBool("walk", true);
                transform.Translate(Vector3.forward * velocidadMovimiento * Time.deltaTime, Space.Self);
                break;
        }
    }

    // --- Vista + ataque ---
    // Devuelve true si está mirando/atacando al jugador (y por tanto anulamos patrulla).
    bool MirarJugadorSiTieneVista()
    {
        if (jugador == null) return false;

        Vector3 ojo = transform.position + Vector3.up * alturaOjos;
        Vector3 cabezaJugador = AproximarCabeza(jugador);
        Vector3 toTarget = cabezaJugador - ojo;

        distActual = toTarget.magnitude;
        if (distActual > distanciaDeteccion) return false;

        // 1) ¿Está delante? (dentro del cono FOV)
        float ang = Vector3.Angle(transform.forward, toTarget);
        if (ang > fov * 0.5f) return false; // fuera del campo de visión

        // 2) ¿Hay línea de visión? (raycast hasta el jugador)
        if (!TieneLineaDeVision(ojo, toTarget.normalized, distActual))
            return false;

        // 3) Girar (solo en Y) hacia el jugador
        Vector3 dirPlano = jugador.position - transform.position;
        dirPlano.y = 0f;
        if (dirPlano.sqrMagnitude > 0.0001f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(dirPlano);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, rotObjetivo, velocidadGiroCuerpo * Time.deltaTime
            );
        }

        if (animator) animator.SetBool("walk", false);

        // 4) Intentar atacar si está dentro del rango
        if (distActual <= rangoAtaque)
            TryAtacar();

        return true;
    }

    void TryAtacar()
    {
        if (Time.time < proximoAtaque) return;
        if (atacando) return;

        atacando = true;
        proximoAtaque = Time.time + tiempoEntreAtaques;

        // Si tienes animación de ataque, dispara Trigger y deja que el evento llame a EjecutarAtaque()
        if (animator != null && HasAnimatorParam(animator, "attack", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger("attack");
        }
        else
        {
            // Fallback: ejecuta inmediatamente
            EjecutarAtaque();
            FinDeAtaque(); // liberar estado
        }
    }

    // Llamar desde un Animation Event en el clip de ataque (en el frame de impacto/disparo)
    public void EjecutarAtaque()
    {
        if (tipoAtaque == TipoAtaque.Ranged)
            DispararProyectil();
        else
            HacerMelee();
    }

    // Llamar desde un Animation Event al final del clip de ataque
    public void FinDeAtaque()
    {
        atacando = false;
    }

    void DispararProyectil()
    {
        if (proyectilPrefab == null || puntoDisparo == null)
        {
            Debug.LogWarning("[Enemigo] Falta proyectilPrefab o puntoDisparo.");
            return;
        }

        // Apuntar exactamente al jugador en el momento del disparo
        Vector3 dir = (AproximarCabeza(jugador) - puntoDisparo.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject go = Instantiate(proyectilPrefab, puntoDisparo.position, rot);
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = dir * velocidadProyectil;

        if (vidaProyectil > 0f)
            Destroy(go, vidaProyectil);
    }

    void HacerMelee()
    {
        // Centro un poco al frente para no pegarnos a nosotros mismos
        Vector3 centro = transform.position + transform.forward * (radioMelee * 0.75f) + Vector3.up * 1.0f;

        Collider[] hits = Physics.OverlapSphere(centro, radioMelee, mascaraMelee, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.CompareTag(tagJugador) || h.transform.root.CompareTag(tagJugador))
            {
                // Ejemplo: buscar un componente de vida del jugador y hacerle daño
                var salud = h.GetComponentInParent<MonoBehaviour>();
                // Reemplaza por tu script real de vida, p.ej. PlayerHealth/SaludJugador:
                // var salud = h.GetComponentInParent<SaludJugador>();
                // if (salud != null) salud.RecibirDanio(danioMelee);

                Debug.Log("[Enemigo] Golpe melee al jugador por " + danioMelee + " de daño.");
                break;
            }
        }
    }

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

    Vector3 AproximarCabeza(Transform t)
    {
        return t.position + Vector3.up * 1.6f;
    }

    // Gizmos para depurar FOV / rangos
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

        // Rango de ataque
        Gizmos.color = tipoAtaque == TipoAtaque.Ranged ? Color.magenta : Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        if (tipoAtaque == TipoAtaque.Melee)
        {
            Gizmos.color = Color.red;
            Vector3 centro = transform.position + transform.forward * (radioMelee * 0.75f) + Vector3.up * 1.0f;
            Gizmos.DrawWireSphere(centro, radioMelee);
        }
    }

    // Utilidad: comprobar si existe el parámetro del Animator (evita errores por nombres)
    bool HasAnimatorParam(Animator anim, string param, AnimatorControllerParameterType type)
    {
        foreach (var p in anim.parameters)
            if (p.type == type && p.name == param) return true;
        return false;
    }
}
