using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgain;

    private void Awake()
    {
        playAgain.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;

        // CASE 1: CLIENT bị kick (client mất kết nối nhưng host vẫn hoạt động)
        if (clientId == localId && clientId != NetworkManager.ServerClientId)
        {
            Debug.Log("Local client has been kicked!");
            Show(); // Hiện UI báo bị kick
            return;
        }

        // CASE 2: SERVER SHUTDOWN (host tắt game)
        if (clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("Server is shut down");
            Show();
        }
    }


    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;

        }
        else if (NetworkManager.Singleton == null)
        {
           Debug.LogWarning("NetworkManager is null in HostDisconnectUI OnDestroy");
        }
    }



}

