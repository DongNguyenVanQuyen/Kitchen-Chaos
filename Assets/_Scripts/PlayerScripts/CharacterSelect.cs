using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private TextMeshPro playerName;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickBtn;

    private void Awake()
    {
        kickBtn.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchentGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            Debug.Log("Kicking player with clientId: " + playerData.clientId);
            KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            KitchentGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        KitchentGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameManager_OnPlayerDataNetworkListChanged;

        CharacterSelectReady.Instance.OnReadyChanged += PlayerSelectReady_OnReadyChanged; ;

        kickBtn.gameObject.SetActive(KitchentGameMultiplayer.Instance.IsServer);

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

            playerName.text = playerData.name.ToString();

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

    private void OnDestroy()
    {
        if (KitchentGameMultiplayer.Instance != null)
        {
            KitchentGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameManager_OnPlayerDataNetworkListChanged;
        }
        if (CharacterSelectReady.Instance != null)
        {
            CharacterSelectReady.Instance.OnReadyChanged -= PlayerSelectReady_OnReadyChanged;
        }
    }
}

