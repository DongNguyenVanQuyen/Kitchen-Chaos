using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO _kitchenObjectSO;

    // Take Item From Counter
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject()) return; 
        // Player is not carrying something
       KitchenObjects.SpawnKitchenObject(_kitchenObjectSO, player);
        InteractServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }
    [ClientRpc]
    private void InteractClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);

    }

}
