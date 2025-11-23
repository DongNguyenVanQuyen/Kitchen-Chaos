using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
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

    // su dung await từ KitchenObjects
    protected override void Awake()
    {
        base.Awake();
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
            AddIngredientServerRpc(KitchentGameMultiplayer.Instance.GetKitchentOnjectSOIndex(kitchenObjectSO));
            return true;

        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchentGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredienAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });

    }
    public List<KitchenObjectSO> GetKitchentObjectSOList()
    {
        return _kitchenObjectSOList;
    }
}
