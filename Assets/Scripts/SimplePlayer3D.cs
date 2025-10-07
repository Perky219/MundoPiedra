using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayer3D : MonoBehaviour
{
    public float speed = 3f;          // velocidad base (ajusta entre 2.5 y 4)
    public float gravity = -9.81f;    // gravedad
    public float jumpHeight = 0f;     // deja 0 si no hay salto

    private CharacterController cc;
    private Vector3 velocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Movimiento simple en plano XZ
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0f, v).normalized;

        // Mueve en dirección local (si quieres global, quita transform.TransformDirection)
        cc.Move(transform.TransformDirection(move) * speed * Time.deltaTime);

        // Gravedad constante
        if (cc.isGrounded && velocity.y < 0)
            velocity.y = -2f;  // mantiene pegado al suelo

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
