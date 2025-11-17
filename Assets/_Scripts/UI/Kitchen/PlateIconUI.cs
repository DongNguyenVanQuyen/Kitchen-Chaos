using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += _plateKitchenObject_OnIngredientAdded;
    }

    private void _plateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredienAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO kitchenObjectSO in _plateKitchenObject.GetKitchentObjectSOList())
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}
