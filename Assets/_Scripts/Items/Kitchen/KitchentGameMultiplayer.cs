using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchentGameMultiplayer : NetworkBehaviour
{
    public static KitchentGameMultiplayer Instance { get; private set; }

    private NetworkList<PlayerData> playerDataNetworkList;
    public List<Color> playerColorList;


    public const int MAX_PLAYER_AMOUNT = 4;

    public event EventHandler OnTryingToJoingGame;
    public event EventHandler OnFailedToJoingGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenListSO kitchenObjectListSO;

    private void Awake()
    {
         Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            playerDataNetworkList.Add(new PlayerData
            {
                clientId = clientId,
                colorId = GetFirstUnusedColorId(),
            });
        };

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started!";
            return;

        }
        if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full!";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoingGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            OnFailedToJoingGame?.Invoke(this, EventArgs.Empty);
        };
        NetworkManager.Singleton.StartClient();

    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenIObjectServerRpc(GetKitchentOnjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnKitchenIObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchentObjectSOFromIndex(kitchenObjectSOIndex);

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObjects kitchenObject = kitchenObjectTransform.GetComponent<KitchenObjects>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
         kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchentOnjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchentObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return kitchenObjectListSO.kitchentObjectSOList[index];
    }

    public KitchenObjectSO GetKitchentObjectSOFromIndex(int kitchentObjectSOIndex)
    {
        return kitchenObjectListSO.kitchentObjectSOList[kitchentObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObjects kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObjects kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObjects>();
        ClearKithcenObjectOnParentClientRpc(networkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    public void ClearKithcenObjectOnParentClientRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObjects kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObjects>();
        kitchenObject.ClearKitchenObjectOnParent();
    }


    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }
    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }
    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ChangePlayerColorServerRpc(int colorId, RpcParams rpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // Màu đã được chọn bởi người chơi khác
            return;
        }
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;

    }

    // Kiểm tra xem màu có sẵn không
    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    // Lấy ID màu đầu tiên chưa được sử dụng
    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
}
