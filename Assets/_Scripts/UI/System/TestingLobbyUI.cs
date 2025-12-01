using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGamebtn;
    [SerializeField] private Button joinGamebtn;

    private void Awake()
    {
        createGamebtn.onClick.AddListener(() =>
        {
            KitchentGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        });
        joinGamebtn.onClick.AddListener(() =>
        {
            KitchentGameMultiplayer.Instance.StartClient();
        });
    }
}

