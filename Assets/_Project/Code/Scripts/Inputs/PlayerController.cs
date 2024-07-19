using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Inputs
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependency Fields")]
        [SerializeField] private InputReader m_input;
        [SerializeField] private GameObject m_CameraAnchor;
        [SerializeField] private CharacterController m_CharacterController;
        
        [Header("Serialized Private Fields")]
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_runSpeed;
        [SerializeField] private float m_rotationSpeed;
        [SerializeField] private float m_verticalRotation;
        [SerializeField] private const float m_MaxVerticalAngle = 83.0f;
        [SerializeField] private const float m_MinVerticalAngle = -83.0f;

        private Vector2 m_moveDirection;
        private Vector2 m_rotationDirection;
        private Vector2 m_scrollDirection;
        private bool m_mouse1Pressed;
        private bool m_middleMousePressed;
        private bool m_mouse2Pressed;
        private bool m_shiftPressed;
        private float m_runSpeedValue;

        private void Start()
        {
            m_mouse1Pressed = false;
            m_middleMousePressed = false;
            m_mouse2Pressed = false;
            m_shiftPressed = false;
            m_runSpeedValue = 1.0f;

            m_input.MovementEvent += HandleMovement;
            m_input.MouseMovementPerformed += HandleLookAround;
            m_input.ShiftPerformed += HandleShift;

            m_input.MouseMovementCanceled += HandleLookAround;
            m_input.ShiftCanceled += HandleShift;
        }

        private void OnDisable()
        {
            //TODO: Unsubscribe to input methods from InputReader
            m_input.MovementEvent -= HandleMovement;
            m_input.MouseMovementPerformed -= HandleLookAround;
            m_input.ShiftPerformed -= HandleShift;

            m_input.MouseMovementCanceled -= HandleLookAround;
            m_input.ShiftCanceled -= HandleShift;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        private void Update()
        {
            Movement();
        }
        private void LateUpdate()
        {
            LookAround();
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
        /////////////////////////////////////////////////////////////////////////////////////////////
        private void Movement()
        {
            if(m_moveDirection == Vector2.zero)
            {
                return;
            }

            Vector3 inputDirection = new Vector3(m_moveDirection.x, 0.0f, m_moveDirection.y);
            
            Quaternion cameraRotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);

            Vector3 moveDirection = cameraRotation * inputDirection;
            Vector3 currentMovement = new Vector3(moveDirection.x, 0.0f, moveDirection.z) * (m_movementSpeed * m_runSpeedValue * Time.deltaTime);
            m_CharacterController.Move(currentMovement);
        }
        private void LookAround()
        {
            float horizontalRotation = m_rotationDirection.x * m_rotationSpeed * Time.deltaTime;
            transform.Rotate(0.0f, horizontalRotation, 0.0f, Space.World);
            m_CameraAnchor.transform.Rotate(0.0f, horizontalRotation, 0.0f, Space.World);

            m_verticalRotation -= m_rotationDirection.y * m_rotationSpeed * Time.deltaTime;
            m_verticalRotation = Mathf.Clamp(m_verticalRotation, m_MinVerticalAngle, m_MaxVerticalAngle);

            m_CameraAnchor.transform.eulerAngles = new Vector3(m_verticalRotation, transform.eulerAngles.y, 0.0f);
        }
    }
}
