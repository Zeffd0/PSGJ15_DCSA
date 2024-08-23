using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Core.DependencyAgents;
using PSGJ15_DCSA.Interfaces;

namespace PSGJ15_DCSA.Inputs
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependency Fields")]
        [SerializeField] private InputReader m_input;
        [SerializeField] private GameObject m_CameraAnchor;
        [SerializeField] private CharacterController m_CharacterController;
        [SerializeField] private DA_GameStates m_DAGameStates;
        
        [Header("Serialized Private Fields")]
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_runSpeed;
        [SerializeField] private float m_rotationSpeed;
        [Header("Jumping Fields")]
        [SerializeField] private float m_jumpSpeed;
        [SerializeField] private float m_jumpPressBufferTime;
        [SerializeField] private float m_coyoteTime;
        [Header("Physics Fields")]
        [SerializeField] private float m_mass;
        [SerializeField] private const float m_MaxVerticalAngle = 83.0f;
        [SerializeField] private const float m_MinVerticalAngle = -83.0f;

        [Header("Placeholder Move to adequate fields after")]

        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Camera cam;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Animations
        // =========================================================================================================

        public const string IDLE = "Idle";
        public const string WALK = "Walk";
        public const string ATTACK1 = "Attack 1";
        public const string ATTACK2 = "Attack 2";
        string currentAnimationState;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Attack
        // =========================================================================================================
        [Header("Attack")]
        public float attackDistance = 3f;
        public float attackDelay = 0.4f;
        public float attackSpeed = 1f;
        public int attackDamage = 1;
        public LayerMask attackLayer;
        public GameObject hitEffect;
        public AudioClip swordSwing;
        public AudioClip hitSound;
        private bool attacking = false;
        private bool readyToAttack = true;
        private int attackCount;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // InputAction Pressed Fields
        // =========================================================================================================
        private Vector3 m_velocity;
        private Vector2 m_moveDirection;
        private Vector3 m_currentMovement;
        private Vector2 m_rotationDirection;
        private Vector2 m_scrollDirection;
        private bool m_mouse1Pressed;
        private bool m_middleMousePressed;
        private bool m_mouse2Pressed;
        private bool m_shiftPressed;
        private bool m_jumpPressed;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Sliding Fields
        // =========================================================================================================

        private float m_runSpeedValue;
        private float m_verticalRotation;
        private bool m_isSliding;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Jumping Fields
        // =========================================================================================================
        private bool m_jumpAttempt;
        private float m_lastJumpPressTime;
        private bool m_wasGrounded;
        private float m_lastGroundedTime;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Misc Important Fields
        // =========================================================================================================
        private bool m_isPlayActive = false;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_DAGameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            //m_DAGameStates.InitGameState(); // todo:: potentially displace this to a class that handles game states globally later
            m_DAGameStates.OnGameStateChanged += HandleToggleActiveInputs;

        }

        private void Start()
        {
            m_mouse1Pressed = false;
            m_middleMousePressed = false;
            m_mouse2Pressed = false;
            m_shiftPressed = false;
            m_jumpPressed = false;
            m_jumpAttempt = false;
            m_isSliding = false;
            m_runSpeedValue = 1.0f;

            m_jumpPressBufferTime = 0.0f;

            m_input.MovementEvent += HandleMovement;
            m_input.MouseMovementPerformed += HandleLookAround;
            m_input.ShiftPerformed += HandleShift;
            m_input.JumpPerformed += HandleJump;
            m_input.Mouse1Performed += StartAttack;
            m_input.Mouse1Canceled += StopAttack;

            m_input.MouseMovementCanceled += HandleLookAround;
            m_input.ShiftCanceled += HandleShift;
            //m_DAGameStates.OnGameStateChanged += HandleToggleActiveInputs;


        }

        private void OnDisable()
        {
            //TODO: Unsubscribe to input methods from InputReader
            m_input.MovementEvent -= HandleMovement;
            m_input.MouseMovementPerformed -= HandleLookAround;
            m_input.ShiftPerformed -= HandleShift;
            m_input.JumpPerformed -= HandleJump;
            m_input.Mouse1Performed += StartAttack;
            m_input.Mouse1Canceled += StopAttack;

            m_input.MouseMovementCanceled -= HandleLookAround;
            m_input.ShiftCanceled -= HandleShift;

            m_DAGameStates.OnGameStateChanged -= HandleToggleActiveInputs;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        private void Update()
        {
            if(m_isPlayActive)
            {
                UpdateGrounded();
                UpdateGravity();
                UpdateMovement();
            }

            if(m_mouse1Pressed)
            {
                Attack();
            }
        }
        private void LateUpdate()
        {
            if(m_isPlayActive)
            {
                UpdateLookAround();   
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        private void HandleMovement(Vector2 direction)
        {
            m_moveDirection = direction;
        }
        private void HandleLookAround(Vector2 direction)
        {
            m_rotationDirection = direction;
        }
        private void HandleShift()
        {
            m_shiftPressed = !m_shiftPressed;
            m_runSpeedValue = m_shiftPressed? m_runSpeed : 1.0f;
        }
        private void HandleJump()
        {
            m_jumpAttempt = true;
            m_lastJumpPressTime = Time.time;
        }

        private void StartAttack()
        {
            m_mouse1Pressed = true;
        }

        private void StopAttack()
        {
            m_mouse1Pressed = false;
        }
        private void HandleToggleActiveInputs(GameState currentGameState)
        {
            //Debug.Log("currentGameState is    " + currentGameState);
            switch(currentGameState)
            {
                case GameState.None:
                case GameState.Initializing:
                case GameState.Loading:
                case GameState.Start:
                {
                    SetBoolGameplayActive(false);
                    m_input.DisableAllInputs();
                }
                break;
                case GameState.Play:
                {
                    SetBoolGameplayActive(true);
                    m_input.SetGameplayInputs();
                    
                }
                break;
                case GameState.Pause:
                case GameState.Menu:
                case GameState.Dead:
                case GameState.GameOver:
                {
                    SetBoolGameplayActive(false);
                    m_input.SetMenuInputs();
                }
                break;
                default:
                {
                    SetBoolGameplayActive(false);
                    m_input.DisableAllInputs();
                }
                break;
            }
        }
        private void SetBoolGameplayActive(bool b)
        {
            m_isPlayActive = b;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateGrounded()
        {
            UpdateSlopeSliding();
            if(m_wasGrounded != m_CharacterController.isGrounded)
            {
                UpdateGroundedTime();
                m_wasGrounded = m_CharacterController.isGrounded;
            }
        }
        private void UpdateSlopeSliding()
        {
            if(m_CharacterController.isGrounded)
            {
                float sphereCastVerticalOffset = m_CharacterController.height / 2 - m_CharacterController.radius;
                Vector3 castOrigin = transform.position - new Vector3(0.0f, sphereCastVerticalOffset, 0.0f);

                int excludeLayers = LayerMask.GetMask("Player") | LayerMask.GetMask("Step");

                if(Physics.SphereCast(castOrigin, m_CharacterController.radius - 0.01f, Vector3.down, out RaycastHit hit, 0.05f, ~excludeLayers, QueryTriggerInteraction.Ignore))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);

                    if(angle >= m_CharacterController.slopeLimit)
                    {
                        Vector3 normal = hit.normal;
                        float yInverse = 1.0f - normal.y;
                        m_velocity.x += yInverse * normal.x;
                        m_velocity.z += yInverse * normal.z;
                    }
                    else
                    {
                        m_velocity.x = 0.0f;
                        m_velocity.z = 0.0f;
                    }
                }
            }
        }
        private void UpdateGroundedTime()
        {
            if(!m_CharacterController.isGrounded)
            {
                m_lastGroundedTime = Time.time;
            }
        }
        private void UpdateGravity()
        {
            Vector3 gravity = Physics.gravity * m_mass * Time.deltaTime;
            m_velocity.y = m_CharacterController.isGrounded ? -1.0f : m_velocity.y + gravity.y;
        }
        private void UpdateMovement()
        {
            UpdateJumpAttempt();
            Vector3 inputDirection = new Vector3(m_moveDirection.x, 0.0f, m_moveDirection.y);
            Quaternion cameraRotation = Quaternion.Euler(0.0f, cam.transform.eulerAngles.y, 0.0f);
            Vector3 moveDirection = cameraRotation * inputDirection;

            m_currentMovement = new Vector3(moveDirection.x, 0.0f, moveDirection.z) * (m_movementSpeed * m_runSpeedValue * Time.deltaTime);
            m_currentMovement += m_velocity * Time.deltaTime;
            m_CharacterController.Move(m_currentMovement);
        }
        private void UpdateJumpAttempt()
        {
            bool wasJumpAttempt = Time.time - m_lastJumpPressTime < m_jumpPressBufferTime;
            bool wasGrounded = Time.time - m_lastGroundedTime < m_coyoteTime;

            bool iswasAttemptingToJump = m_jumpAttempt || (wasJumpAttempt && m_CharacterController.isGrounded);
            bool iswasGrounded = m_CharacterController.isGrounded || wasGrounded;

            if(iswasAttemptingToJump && iswasGrounded)
            {
                m_velocity.y += m_jumpSpeed;
            }
            m_jumpAttempt = false;
        }
        private void UpdateLookAround()
        {
            float horizontalRotation = m_rotationDirection.x * m_rotationSpeed * Time.deltaTime;
            transform.Rotate(0.0f, horizontalRotation, 0.0f, Space.World);
            m_CameraAnchor.transform.Rotate(0.0f, horizontalRotation, 0.0f, Space.World);

            m_verticalRotation -= m_rotationDirection.y * m_rotationSpeed * Time.deltaTime;
            m_verticalRotation = Mathf.Clamp(m_verticalRotation, m_MinVerticalAngle, m_MaxVerticalAngle);

            m_CameraAnchor.transform.eulerAngles = new Vector3(m_verticalRotation, transform.eulerAngles.y, 0.0f);
        }

        public void ChangeAnimationState(string newState) 
        {
            // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
            if (currentAnimationState == newState) return;

            // PLAY THE ANIMATION //
            currentAnimationState = newState;
            animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
        }

        void SetAnimations()
        {
            // If player is not attacking
            if(!attacking)
            {
                if(m_velocity.x == 0 &&m_velocity.z == 0)
                { ChangeAnimationState(IDLE); }
                else
                { ChangeAnimationState(WALK); }
            }
        }

        // ------------------- //
        // ATTACKING BEHAVIOUR //
        // ------------------- //

        public void Attack()
        {
            if(!readyToAttack || attacking) return;

            readyToAttack = false;
            attacking = true;

            Invoke(nameof(ResetAttack), attackSpeed);
            Invoke(nameof(AttackRaycast), attackDelay);

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(swordSwing);

            if(attackCount == 0)
            {
                ChangeAnimationState(ATTACK1);
                attackCount++;
            }
            else
            {
                ChangeAnimationState(ATTACK2);
                attackCount = 0;
            }
        }

        void ResetAttack()
        {
            attacking = false;
            readyToAttack = true;
        }

        void AttackRaycast()
        {
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
            { 
                HitTarget(hit.point);

                if(hit.collider.TryGetComponent(out IDamageable damageable))
                { 
                    damageable.TakeDamage(attackDamage);
                }
            } 
        }

        void HitTarget(Vector3 pos)
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(hitSound);

            GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
            Destroy(GO, 20);
        }
    }
}
