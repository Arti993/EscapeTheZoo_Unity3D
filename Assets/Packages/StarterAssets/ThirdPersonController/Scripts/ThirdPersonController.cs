using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerPickUpOrDrop))]
    [RequireComponent(typeof(PlayerThrowAttack))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private AudioSource _jumpSound;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.4f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.4f)]
        public float RotationSmoothTime = 0.4f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.3f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDDying;
        private int _animIDHolding;
        private int _animIDDrowning;
        private int _animIDThrowAttack;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _characterController;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private PlayerPickUpOrDrop _playerPickUpOrDrop;
        private PlayerThrowAttack _playerThrowAttack;

        private const float _threshold = 0.01f;

        private float _turtleReboundMultiplier = 1.8f;
        private bool _hasAnimator;
        private bool _isMovementActive;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        public static ThirdPersonController Instance;

        public bool IsShootingEnable => _playerThrowAttack.enabled;

        public bool IsPickUpOrDropEnable => _playerPickUpOrDrop.enabled;

        public bool IsPlayerHitByShark { get; private set; } = false;

        public void DisableInput()
        {
            _playerInput.DeactivateInput();
        }

        public void EnableInput()
        {
            _playerInput.ActivateInput();
        }

        public void StopMovement()
        {
            _isMovementActive = false;
        }

        public void StartMovement()
        {
            _isMovementActive = true;
        }

        public void TurnOnControlAfterFixedUpdate()
        {
            StartCoroutine(WaitFixedUpdateToEnableInputAndMovement());
        }

        public void EnableShooting()
        {
            _playerThrowAttack.enabled = true;
        }

        public void DisableShooting()
        {
            _playerThrowAttack.enabled = false;
        }

        public void EnablePickUpOrDrop()
        {
            _playerPickUpOrDrop.enabled = true;
            _playerPickUpOrDrop.EnablePickUpOrDropButton();
        }

        public void DisablePickUpOrDrop()
        {
            _playerPickUpOrDrop.enabled = false;
            _playerPickUpOrDrop.DisablePickUpOrDropButton();
        }

        public void DrownInWater()
        {
            StopMovement();

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDDrowning, true);
            }

            LevelsChanger.Instance.CurrentLevel.Restart();
        }

        public IEnumerator MoveAfterSharkHit(Shark shark)
        {
            float currentMoveTime = 0;
            float maxMoveTime = 0.3f;
            float moveSpeed = 10;

            IsPlayerHitByShark = true;

            DisableInput();
            StopMovement();

            Vector3 moveDirection = shark.transform.forward;
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

            while (currentMoveTime < maxMoveTime)
            {
                transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
                currentMoveTime += Time.deltaTime;

                yield return null;
            }

            IsPlayerHitByShark = false;

            StartMovement();
            EnableInput();
        }

        private void Awake()
        {
            Instance = this;
            _isMovementActive = true;

            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _playerPickUpOrDrop = GetComponent<PlayerPickUpOrDrop>();
            _playerThrowAttack = GetComponent<PlayerThrowAttack>();

            DisablePickUpOrDrop();
            DisableShooting();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _characterController = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            LevelsChanger.Instance.CurrentLevel.AllActionsFinished += OnCurrentLevelAllActionsFinished;
            LevelsChanger.Instance.CurrentLevel.Restarted += OnCurrentLevelRestarted;
            _playerPickUpOrDrop.ItemTaken += OnPickUp;
            _playerPickUpOrDrop.ItemDropped += OnDrop;
            _playerThrowAttack.Attacked += OnAttack;
            _player.Dying += OnDying;


#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void OnDestroy()
        {
            LevelsChanger.Instance.CurrentLevel.AllActionsFinished -= OnCurrentLevelAllActionsFinished;
            LevelsChanger.Instance.CurrentLevel.Restarted -= OnCurrentLevelRestarted;
            _playerPickUpOrDrop.ItemTaken -= OnPickUp;
            _playerPickUpOrDrop.ItemDropped -= OnDrop;
            _playerThrowAttack.Attacked -= OnAttack;
            _player.Dying -= OnDying;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            if (_isMovementActive)
            {
                Move();
            }

            JumpAndGravity();
            GroundedCheck();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDying = Animator.StringToHash("IsDead");
            _animIDHolding = Animator.StringToHash("IsHolding");
            _animIDDrowning = Animator.StringToHash("IsDrown");
            _animIDThrowAttack = Animator.StringToHash("IsThrow");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }

                    _jumpSound.Play();
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
 
        }

        private void OnLand(AnimationEvent animationEvent)
        {

        }

        private void OnAttack()
        {
            if(_playerThrowAttack.enabled == true)
            {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDThrowAttack, true);
                    }
            }
        }

        private void OnPickUp()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHolding, true);
            }
        }

        private void OnDrop()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDHolding, false);
            }
        }

        private void OnDying()
        {
            StopMovement();

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDDying, true);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.TryGetComponent(out Turtle turtle))
            {
                ReboundOnTurtle();
                turtle.CollisionWithPlayer();
            }    
        }

        private void ReboundOnTurtle()
        {
            _verticalVelocity = _turtleReboundMultiplier * Mathf.Sqrt(JumpHeight * -2f * Gravity);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, true);
            }
        }

        private void OnCurrentLevelAllActionsFinished()
        {
            LevelsChanger.Instance.PreviousLevel.AllActionsFinished -= OnCurrentLevelAllActionsFinished;
            LevelsChanger.Instance.PreviousLevel.Restarted -= OnCurrentLevelRestarted;
            LevelsChanger.Instance.CurrentLevel.AllActionsFinished += OnCurrentLevelAllActionsFinished;
            LevelsChanger.Instance.CurrentLevel.Restarted += OnCurrentLevelRestarted;
        }

        private void OnCurrentLevelRestarted()
        {
            if (_animator.GetBool(_animIDDying))
                _animator.SetBool(_animIDDying, false);

            if (_animator.GetBool(_animIDDrowning))
                _animator.SetBool(_animIDDrowning, false);               
        }



        private IEnumerator WaitFixedUpdateToEnableInputAndMovement()
        {
            yield return new WaitForFixedUpdate();

            StartMovement();
            EnableInput();
        }
    }
}