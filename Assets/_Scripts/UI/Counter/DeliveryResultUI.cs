using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Sprite successIconSprite;
    [SerializeField] private Sprite failedIconSprite;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor; 

    private void Start()
    {
        Hide();
        DeliverManager.Instance.OnRecipeFailed += DeliverManager_OnRecipeFailed;
        DeliverManager.Instance.OnRecipeSuccess += DeliverManager_OnRecipeSuccess;
    }

    private void DeliverManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        ShowResult(successIconSprite, "DELIVERY\nSUCCESS", successColor);
    }

    private void DeliverManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        ShowResult(failedIconSprite, "DELIVERY\nFAILED", failedColor);
    }
    private void ShowResult(Sprite iconSprite, string message, Color backgroundColor)
    {
        gameObject.SetActive(true);
        iconImage.sprite = iconSprite;
        messageText.text = message;
        backgroundImage.color = backgroundColor;    
        // Optionally, you can add a timer to hide the UI after a few seconds
        // Invoke("Hide", 2f);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
