using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;


    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurnedRecipeSO[] burningRecipeSOArray;

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private FryingRecipeSO _fryingRecipeSO;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    [SerializeField] private BurnedRecipeSO _burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTime_OnValueChanged;
        burningTimer.OnValueChanged += BurningTime_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }
    private void FryingTime_OnValueChanged(float previousValue, float newValue)
    {
        // Handle frying timer value change if needed

        float fryingTimerMax = _fryingRecipeSO != null ? _fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / _fryingRecipeSO.fryingTimerMax
        });
    }
    private void BurningTime_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = _burningRecipeSO != null ? _burningRecipeSO.burnedTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / _burningRecipeSO.burnedTimerMax
        });
    }
    private void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });
        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f

            });
        }

    }

    private void Start()
    {
        state.Value = State.Idle;
    }

    private void Update()
    {
        if (!IsServer) return;
        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:

                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > _fryingRecipeSO.fryingTimerMax)
                    {
                        // Fried
                        KitchenObjects.DestroyKitchenObject(GetKitchenObject()); 
                        KitchenObjects.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        Debug.Log("Object fried");
                        state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        // Set Burning for Client
                        SetBurningRecipeSOClientRpc(KitchentGameMultiplayer.Instance.GetKitchentOnjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;
                   
                    if (burningTimer.Value > _burningRecipeSO.burnedTimerMax)
                    {
                        // Fried
                        KitchenObjects.DestroyKitchenObject(GetKitchenObject());

                        KitchenObjects.SpawnKitchenObject(_burningRecipeSO.output, this);

                        Debug.Log("Object Burned");
                        state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
            }
            Debug.Log(state);
        }   
    }

    public override void Interact(Player player)
    {
        if (!this.HasKitchenObject())
        {
            // There is no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player carrying something that can be Fried
                    KitchenObjects kitchenObjects = player.GetKitchenObject();
                    kitchenObjects.SetKitchenObjectParent(this);

                    InteractPlateServerRpc(KitchentGameMultiplayer.Instance.GetKitchentOnjectSOIndex(kitchenObjects.GetKitchenObjectSO()));

                }
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
                    if (plateKitchenObject.TryAddIngredient(this.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchentGameMultiplayer.Instance.DestroyKitchenObject(this.GetKitchenObject());
                        SetStateIdleServerRpc();

                    }
                }
            }
            else
            {
                // take item from counter
                // Player is not carrying anything
                this.GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;

    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void InteractPlateServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchentGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }
    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchentGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurnedRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurnedRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
    public bool IsFrying()
    {
        return state.Value == State.Frying;
    }
    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
