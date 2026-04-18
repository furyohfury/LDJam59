using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public sealed class PlayerController : Singleton<PlayerController>, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _actions;

        protected override void Awake()
        {
            base.Awake();
            _actions = new InputSystem_Actions();
            _actions.Player.SetCallbacks(this);
            _actions.Player.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            Vector2Int direction;

            // Получаем сырой Vector2 (например, 0.7, 0.7 при зажатых W и D)
            Vector2 rawInput = context.ReadValue<Vector2>();

            // Ограничиваем ввод: выбираем доминирующую ось, чтобы не ходить по диагонали
            if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
            {
                direction = new Vector2Int(rawInput.x > 0 ? 1 : -1, 0);
            }
            else if (Mathf.Abs(rawInput.y) > 0)
            {
                direction = new Vector2Int(0, rawInput.y > 0 ? 1 : -1);
            }
            else
            {
                direction = Vector2Int.zero;
            }

            Player.Instance.Move(direction);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
        }

        private void OnDestroy()
        {
            _actions.Player.Disable();
        }
    }
}
