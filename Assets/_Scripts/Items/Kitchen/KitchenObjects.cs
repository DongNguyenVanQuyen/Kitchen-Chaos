using UnityEngine;

public class KitchenObjects : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
  
    [SerializeField] private IKitchenObjectParent _kitchenObjectParent;
    
    public KitchenObjectSO GetKitchenObjectSO() => _kitchenObjectSO;

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (this._kitchenObjectParent != null)
            this._kitchenObjectParent.ClearKitchenObject();

        this._kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("Counter already has a kitchen object." + kitchenObjectParent.GetKitchenObject());
        }
        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }
    
    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;

    public void DestroySelf()
    {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public static KitchenObjects SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObjects kitchenObject = kitchenObjectTransform.GetComponent<KitchenObjects>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }
}
