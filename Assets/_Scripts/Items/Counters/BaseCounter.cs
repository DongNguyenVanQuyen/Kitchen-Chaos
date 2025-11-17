using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    public static event EventHandler OnAnyObjectPlacedHere;

    [SerializeField] private Transform _counterTopPoint;

    [SerializeField] private KitchenObjects _kitchenObject;


    // Player Check Selected Counter
    public virtual void Interact(Player player)
    {
        Debug.Log("BaseCounter Interact");
    }
    public virtual void InteractAlternate(Player player)
    {
        Debug.Log("BaseCounter Interact");
    }

    public Transform GetKitchenObjectFollowTransform() => _counterTopPoint;

    public void SetKitchenObject(KitchenObjects kitchenObject)
    {
        this._kitchenObject = kitchenObject;
        if (this._kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObjects GetKitchenObject() => _kitchenObject;

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() => _kitchenObject != null;
}
