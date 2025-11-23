using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;

    // Put item on counter
    public override void Interact(Player player)
    {
        if (!this.HasKitchenObject())
        {
            // There is no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player is not carrying anything
                
            }
        }
        else
        {
            // There is KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchentGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    // Player is not carrying Plate but something else
                    // plateKitchenObject is valid -> no need PlateKitchenObject
                    if (this.GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())){
                            KitchentGameMultiplayer.Instance.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }

            }
            else
            {
                // take item from counter
                // Player is not carrying anything
                this.GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }


}
