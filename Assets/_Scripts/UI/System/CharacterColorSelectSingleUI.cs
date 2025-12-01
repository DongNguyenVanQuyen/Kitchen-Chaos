using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchentGameMultiplayer.Instance.ChangePlayerColor(colorId);
            UpdateIsSelected();
        });
    }
    private void Start()
    {
        KitchentGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged; 
        UpdateUI();
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();

    }

    private void UpdateUI()
    {
        image.color = KitchentGameMultiplayer.Instance.GetPlayerColor(colorId);
    }

    private void UpdateIsSelected()
    {
        if (KitchentGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selected.SetActive(true);
        }
        else
        {
            selected.SetActive(false);
        }
    }

}
