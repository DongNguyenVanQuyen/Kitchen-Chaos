using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinCodeBtn;
    [SerializeField] private Button joinLobbyBtn;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform template;

    private void Awake()
    {
        mainMenuBtn.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        createLobbyBtn.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        joinLobbyBtn.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });
        joinCodeBtn.onClick.AddListener(() =>
        {
            string joinCode = joinCodeInputField.text;
            KitchenGameLobby.Instance.JoinWithCode(joinCode);
        });
        template.gameObject.SetActive(false);
    }

    private void Start()
    {
       playerNameInputField.text = KitchentGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newName) =>
        {
          KitchentGameMultiplayer.Instance.SetPlayerName(newName);
        });
        KitchenGameLobby.Instance.OnLobbListChanged += KitchenGameManager_OnLobbListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void KitchenGameManager_OnLobbListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == template) continue;
            Destroy(child.gameObject);
        }
        
        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(template, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbySingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbListChanged -= KitchenGameManager_OnLobbListChanged;
    }
}


