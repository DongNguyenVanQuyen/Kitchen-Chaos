using UnityEngine;

[CreateAssetMenu()]
public class BurnedRecipeSO : ScriptableObject 
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burnedTimerMax;
}
