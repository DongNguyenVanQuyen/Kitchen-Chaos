using Unity.Netcode;
using UnityEngine;

public class KitchenObjects : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
  
    [SerializeField] private IKitchenObjectParent _kitchenObjectParent;
    
    public KitchenObjectSO GetKitchenObjectSO() => _kitchenObjectSO;

    public FollowTransform followTransform;

    // Cho plates su dung chung
    protected virtual void Awake()
    {
        if (followTransform == null)
            followTransform = GetComponent<FollowTransform>();
    }
        
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentRpc(kitchenObjectParent.GetNetworkObject());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetKitchenObjectParentRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
            SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this._kitchenObjectParent != null)
            this._kitchenObjectParent.ClearKitchenObject();

        this._kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("Counter already has a kitchen object." + kitchenObjectParent.GetKitchenObject());
        }
        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());

    }
    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void ClearKitchenObjectOnParent()
    {
        _kitchenObjectParent.ClearKitchenObject();

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

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchentGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObjects kitchenObject)
    {
        KitchentGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}
