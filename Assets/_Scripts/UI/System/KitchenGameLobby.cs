using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.FilePathAttribute;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinLobbyStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public System.Collections.Generic.List<Lobby> lobbyList;
    }


    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHearbeat();
        HandlePeriodicListLobbies();
    }
    private async Task HandlePeriodicListLobbies()
    {
        if (joinedLobby != null && AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
        {
            return;
        }
        listLobbiesTimer -= Time.deltaTime;
        if (listLobbiesTimer <= 0f)
        {
            listLobbiesTimer = 3f;
             await ListLobbies();
        }
    }
    private void HandleHearbeat()
    {
        if (joinedLobby == null)
        {
            return;
        }
        if (isLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                heartbeatTimer = 15f;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    private bool isLobbyHost()
    {
        return AuthenticationService.Instance.PlayerId == joinedLobby.HostId && joinedLobby != null;
    }

    private async Task ListLobbies()
    {
        try
        {
            var queryOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };

            QueryResponse result = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

            OnLobbListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = result.Results,
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }


    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchentGameMultiplayer.MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
        return default;
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;    
        }catch(RelayServiceException e)
        {
            Debug.LogError(e);
        }   
        return default;
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
        return default;
    }

    public async Task CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchentGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    {
                        KEY_RELAY_JOIN_CODE,
                        new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: relayJoinCode)
                    }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            KitchentGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async Task QuickJoin()
    {
        OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchentGameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty); 
        }
    }
    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    public async Task JoinWithCode(string lobbyCode)
    {
        OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            // 1. Join Lobby bằng mã
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            // 2. Lấy Relay Join Code từ lobby data
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            // 3. Join Relay
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            // 4. Gán Relay server data vào UnityTransport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // 5. Start Netcode client
            KitchentGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogError(e);
        }
    }

    public async Task JoinWithId(string lobbyId)
    {
        OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            // 2. Lấy RelayJoinCode từ lobby
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            // 3. Join Relay
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            // 4. Set Relay server data
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchentGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);

            Debug.LogError(e);
        }
    }
    public async Task DeleteLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

            joinedLobby = null;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task LeaveLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async Task KickPlayer(string playerId)
    {
        if (!isLobbyHost())
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
} 
