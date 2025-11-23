using Unity.Netcode;
using UnityEngine;

public class KitchentGameMultiplayer : NetworkBehaviour
{
    public static KitchentGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenListSO kitchenObjectListSO;

    private void Awake()
    {
         Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenIObjectServerRpc(GetKitchentOnjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnKitchenIObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchentObjectSOFromIndex(kitchenObjectSOIndex);

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObjects kitchenObject = kitchenObjectTransform.GetComponent<KitchenObjects>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
         kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchentOnjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchentObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        return kitchenObjectListSO.kitchentObjectSOList[index];
    }

    public KitchenObjectSO GetKitchentObjectSOFromIndex(int kitchentObjectSOIndex)
    {
        return kitchenObjectListSO.kitchentObjectSOList[kitchentObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObjects kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObjects kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObjects>();
        ClearKithcenObjectOnParentClientRpc(networkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    public void ClearKithcenObjectOnParentClientRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObjects kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObjects>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
}
