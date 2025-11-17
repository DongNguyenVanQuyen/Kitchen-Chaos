using System;
using UnityEngine;
using UnityEngine.EventSystems;

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
        
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }

}
