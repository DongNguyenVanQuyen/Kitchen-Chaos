using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI soundEffectsText;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;


    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    private void Awake()
    {
        Instance = this;

        resetButton.onClick.AddListener(() =>
        {
            GameInput.Instance.ResetBindings();
            UpdateVisual();
        });

        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            GamePauseUI.Instance.Show();
        });
        moveUpButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Move_Up, UpdateVisual);
        });
        moveDownButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Move_Down, UpdateVisual);
        });
        moveLeftButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Move_Left, UpdateVisual);
        });
        moveRightButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Move_Right, UpdateVisual);
        });
        interactButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Interact, UpdateVisual);
        });
        interactAlternateButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.InteractAlternate, UpdateVisual);
        });
        pauseButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Pause, UpdateVisual);
        });
        gamepadInteractButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Gamepad_Interact, UpdateVisual);
        });
        gamepadInteractAlternateButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Gamepad_InteractAlternate, UpdateVisual);
        });
        gamepadPauseButton.onClick.AddListener(() =>
        {
            GameInput.Instance.RebindBinding(GameInput.Binding.Gamepad_Pause, UpdateVisual);
        });


    }
    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnPaused += KitchenGameManager_OnGameUnPaused; ;

        Hide();
        UpdateVisual();
    }

    private void KitchenGameManager_OnGameUnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10).ToString();

        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10).ToString();
        
        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        musicButton.Select();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}