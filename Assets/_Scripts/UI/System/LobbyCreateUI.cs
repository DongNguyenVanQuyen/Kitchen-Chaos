using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button createPublicBtn;
    [SerializeField] private Button createPrivateBtn;
    [SerializeField] private TMP_InputField LobbyNameInputField;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide();
        });
        createPublicBtn.onClick.AddListener(() =>
        {
            string lobbyName = LobbyNameInputField.text;
            KitchenGameLobby.Instance.CreateLobby(lobbyName, isPrivate: false);
        });
        createPrivateBtn.onClick.AddListener(() =>
        {
            string lobbyName = LobbyNameInputField.text;
            KitchenGameLobby.Instance.CreateLobby(lobbyName, isPrivate: true);
        });
    }
    private void Start()
    {
        Hide();
    
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}

