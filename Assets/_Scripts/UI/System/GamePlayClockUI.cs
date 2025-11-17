using UnityEngine;
using UnityEngine.UI;

public class GamePlayClockUI : MonoBehaviour
{
    [SerializeField] private Image _timerImage;

    private void Update()
    {
        _timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
