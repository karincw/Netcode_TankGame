using System;
using UnityEngine;
using static PlayerInput;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(fileName = "Karin/InputSO")]
public class InputSO : ScriptableObject, IPlayerActions, IUIActions
{
    #region Event Section

    public event Action<Vector2> OnMovementEvent;
    public event Action<bool> OnFireEvent;
    public event Action OnDashEvent;

    #endregion
    #region Velue Section

    #endregion

    private PlayerInput _inputSystem;
    public PlayerInput InputSystem => _inputSystem;

    private void OnEnable()
    {
        if (_inputSystem == null)
        {
            _inputSystem = new PlayerInput();
            _inputSystem.Player.SetCallbacks(this);
            _inputSystem.UI.SetCallbacks(this);
        }

        _inputSystem.Player.Enable();
        //_inputSystem.UI.Enable();
    }

    #region PlayerAction

    public void OnMovement(CallbackContext context)
    {
        //선함쌤한테 가기
        //move로 바뀌는거에서 이벤트가 한번호출되고 move로 넘어가는데
        //Move상태에서 이벤트가 실행이안되서 이벤트에 direction이 안바뀌고 그래서 안움직임 ㅇㅇ

        //Debug.Log("EventCall");
        if (context.performed)
        {
            //Debug.Log("EventInvoke");
            Vector2 movement = context.ReadValue<Vector2>();
            OnMovementEvent?.Invoke(movement);
        }
        if(context.canceled)
        {
            OnMovementEvent?.Invoke(Vector2.zero);
        }
    }

    public void OnFire(CallbackContext context)
    {
        if (context.performed)
        {
            OnFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnFireEvent?.Invoke(false);
        }
    }

    public void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            OnDashEvent?.Invoke();
        }
    }

    #endregion
}
