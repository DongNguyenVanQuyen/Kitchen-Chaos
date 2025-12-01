using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{


    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
           Destroy(NetworkManager.Singleton.gameObject);
        }
        if (KitchentGameMultiplayer.Instance != null)
        {
            Destroy(KitchentGameMultiplayer.Instance.gameObject);
        }
        if (KitchenGameLobby.Instance != null)
        {
            Destroy(KitchenGameLobby.Instance.gameObject);
        }
    }
}
