using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button startServerButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            var networkManager = Unity.Netcode.NetworkManager.Singleton;
            if (networkManager != null)
            {
                Debug.Log("Starting Host...");
                networkManager.StartHost();
                Hide();
            }
        });
        startClientButton.onClick.AddListener(() =>
        {
            var networkManager = Unity.Netcode.NetworkManager.Singleton;
            if (networkManager != null)
            {
                Debug.Log("Starting Client...");
                networkManager.StartClient();
                Hide();
            }
        });
        startServerButton.onClick.AddListener(() =>
        {
            var networkManager = Unity.Netcode.NetworkManager.Singleton;
            if (networkManager != null)
            {
                Debug.Log("Starting Server...");
                networkManager.StartServer();
                Hide();
            }
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
