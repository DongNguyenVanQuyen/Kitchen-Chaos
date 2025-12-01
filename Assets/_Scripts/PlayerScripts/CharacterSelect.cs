using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;

    private void Start()
    {
        KitchentGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameManager_OnPlayerDataNetworkListChanged;

        CharacterSelectReady.Instance.OnReadyChanged += PlayerSelectReady_OnReadyChanged; ;

        UpdatePlayer();
    }

    private void PlayerSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchentGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Debug.Log("Showing player index: " + playerIndex);
            Show();
            PlayerData playerData = KitchentGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerIndexReady(playerData.clientId));

            playerVisual.SetPlayerColor(KitchentGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Debug.Log("Hiding player index: " + playerIndex);
            Hide();
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
}
