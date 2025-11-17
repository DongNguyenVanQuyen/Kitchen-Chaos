using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObjects
{
    public event EventHandler<OnIngredienAddedEventArgs> OnIngredientAdded;
    public class OnIngredienAddedEventArgs: EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchentObjectSO;

    private List<KitchenObjectSO> _kitchenObjectSOList;
     
    private void Awake()
    {
        _kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchentObjectSO.Contains(kitchenObjectSO))
        {
            return false;
        }
        if (_kitchenObjectSOList.Contains(kitchenObjectSO)){
            return false;
        }
        else
        {
            _kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredienAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchentObjectSOList()
    {
        return _kitchenObjectSOList;
    }
}
