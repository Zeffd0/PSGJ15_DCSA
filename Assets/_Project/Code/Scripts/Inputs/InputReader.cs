using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSGJ15_DCSA.Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Project/Input Reader", order = 1)]
    public class InputReader : ScriptableObject, PlayerInputs.IGameplayActions, PlayerInputs.IMenuActions
    {
        private PlayerInputs m_PlayerInputs;
        #region Gameplay Inputs
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Movement Actions | Vector2
        // =========================================================================================================
        public event Action<Vector2> MovementEvent;
        public void OnMovement(InputAction.CallbackContext context)
        {
            //Debug.Log($"'OnMovement' context : {context}");
            MovementEvent?.Invoke(context.ReadValue<Vector2>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MouseMovement Actions | Vector2
        // =========================================================================================================
        public event Action<Vector2> MouseMovementPerformed;
        public event Action<Vector2> MouseMovementCanceled;
        public void OnLookAround(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                MouseMovementPerformed?.Invoke(context.ReadValue<Vector2>());
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                MouseMovementCanceled?.Invoke(context.ReadValue<Vector2>());
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Mouse1 Actions | Float
        // =========================================================================================================
        public event Action Mouse1Performed;
        public event Action Mouse1Canceled;
        public void OnMouse1(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                Mouse1Performed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                Mouse1Canceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MiddleMouse Actions | Float
        // =========================================================================================================
        public event Action MiddleMousePerformed;
        public event Action MiddleMouseCanceled;
        public void OnMiddleMouse(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                MiddleMousePerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                MiddleMouseCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Mouse2 Actions | Float
        // =========================================================================================================
        public event Action Mouse2Performed;
        public event Action Mouse2Canceled;
        public void OnMouse2(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                Mouse2Performed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                Mouse2Canceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MouseScroll | Delta Vector2.y
        // =========================================================================================================
        public event Action<Vector2> MouseScrollEvent;
        public void OnMouseScroll(InputAction.CallbackContext context)
        {
            MouseScrollEvent?.Invoke(context.ReadValue<Vector2>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Shift | Float
        // =========================================================================================================
        public event Action ShiftPerformed;
        public event Action ShiftCanceled;
        public void OnShift(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                ShiftPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                ShiftCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Jump | Float
        // =========================================================================================================
        public event Action JumpPerformed;
        public event Action JumpCanceled;
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                JumpPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                JumpCanceled?.Invoke();
            }
        }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Mouse2 Actions | Float
        // =========================================================================================================
        public event Action HorizontalSlashPerformed;
        public event Action HorizontalSlashCancelled;
        public void OnHorizontalSlash(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                HorizontalSlashPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                HorizontalSlashCancelled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Menu Inputs
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TODO: Fill up once Menu actions have been established
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Methods
        // =========================================================================================================
        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            if(m_PlayerInputs == null)
            {
                m_PlayerInputs = new PlayerInputs();
                m_PlayerInputs.Gameplay.SetCallbacks(this);
                m_PlayerInputs.Menu.SetCallbacks(this);

                // Might need to get rid of this once we have menu up and running and enable MenuInputs once starting up
                //SetGameplayInputs();
                SetMenuInputs();
            }
        }
        private void OnDisable()
        {
            DisableAllInputs();
            // #if UNITY_EDITOR
            // UnityEditor.EditorApplication.update += ResetCursor;
            // #endif
        }
        public void SetGameplayInputs()
        {
            SetCursorToPlayMode();
            m_PlayerInputs.Gameplay.Enable();
            m_PlayerInputs.Menu.Disable();
        }
        public void SetMenuInputs()
        {
            SetCursorToMenuMode();
            m_PlayerInputs.Gameplay.Disable();
            m_PlayerInputs.Menu.Enable();
        }
        public void DisableAllInputs()
        {
            m_PlayerInputs.Gameplay.Disable();
            m_PlayerInputs.Menu.Disable();
        }
        private void SetCursorToPlayMode()
        {
            //SetCursorToMenuMode(); // ???
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void SetCursorToMenuMode()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        void ResetCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= ResetCursor;
            #endif
        }
        #endregion
    }
}
