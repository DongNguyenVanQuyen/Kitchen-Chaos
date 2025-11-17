using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }
    [SerializeField] private PlayerInputActions _playerInputActions;
   
    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        else
        {
            _playerInputActions = new PlayerInputActions();
        }

        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += Interact_prefomed;
        
        _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_prefomed;

        _playerInputActions.Player.Pause.performed += Pause_performed; ;
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= Interact_prefomed;
        _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_prefomed;
        _playerInputActions.Player.Pause.performed -= Pause_performed;
        _playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    // F action is triggered when the Alternate Interact button is pressed
    private void InteractAlternate_prefomed(InputAction.CallbackContext context)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    // E action is triggered when the Interact button is pressed
    private void Interact_prefomed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 InputMovement = _playerInputActions.Player.Move.ReadValue<Vector2>();
       
        InputMovement = InputMovement.normalized;

        return InputMovement;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();

        }
    }
    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        InputAction inputAction;
        int bindingIndex;
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;

        }
        inputAction.Disable();
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                inputAction.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson()); 
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }

    public void ResetBindings()
    {
        // Xóa toàn bộ binding overrides đã lưu trong PlayerPrefs
        PlayerPrefs.DeleteKey(PLAYER_PREFS_BINDINGS);
        PlayerPrefs.Save();

        // Reset trong runtime (bắt buộc để áp dụng ngay)
        _playerInputActions.RemoveAllBindingOverrides();

        Debug.Log("Đã reset tất cả phím về mặc định");
    }

}
