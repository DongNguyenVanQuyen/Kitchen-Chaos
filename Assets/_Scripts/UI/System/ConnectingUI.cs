using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
   
    private void Start()
    {
        KitchentGameMultiplayer.Instance.OnTryingToJoingGame += KitchenGameMultiplayer_OnTryingToJoingGame;
        KitchentGameMultiplayer.Instance.OnFailedToJoingGame += KitchenGameMultiplayer_OnFailedToJoingGame;
        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoingGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoingGame(object sender, System.EventArgs e)
    {
        Show();
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
        KitchentGameMultiplayer.Instance.OnTryingToJoingGame -= KitchenGameMultiplayer_OnTryingToJoingGame;
        KitchentGameMultiplayer.Instance.OnFailedToJoingGame -= KitchenGameMultiplayer_OnFailedToJoingGame;
    }
}
