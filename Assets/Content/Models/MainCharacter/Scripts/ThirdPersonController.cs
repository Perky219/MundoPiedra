using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player Movement")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;

        [Header("Gravity")]
        public float Gravity = -15.0f;

        [Header("Rotation")]
        public float RotationSensitivity = 5f;  // sensibilidad de rotación con el mouse

        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private bool _hasAnimator;

        private int _animIDHorizontal;
        private int _animIDVertical;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            AssignAnimationIDs();
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            ApplyGravity();
            RotateWithMouse();
            Move();
        }

        private void AssignAnimationIDs()
        {
            _animIDHorizontal = Animator.StringToHash("Horizontal");
            _animIDVertical = Animator.StringToHash("Vertical");
        }

        private void RotateWithMouse()
        {
            // rotar al personaje con el mouse en el eje Y
            float mouseX = _input.look.x;
            transform.Rotate(Vector3.up * mouseX * RotationSensitivity);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // WASD siempre relativo al personaje
            Vector3 move = transform.right * _input.move.x + transform.forward * _input.move.y;

            _controller.Move(move.normalized * (targetSpeed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDHorizontal, _input.move.x);
                _animator.SetFloat(_animIDVertical, _input.move.y);
            }
        }

        private void ApplyGravity()
        {
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
    }
}
