using UnityEngine;

public class AntBehaviour : MonoBehaviour
{
    protected virtual void LoadComponents()
    {
        // Load components that are common to all ants
        // This method can be overridden in derived classes to load additional components
        // or perform specific initialization tasks.
    }
    protected virtual void LoadAttributes()
    {
        // Load attributes that are common to all ants
        // This method can be overridden in derived classes to load additional attributes
        // or perform specific initialization tasks.
    }
    protected virtual void Reset()
    {
        LoadComponents();
        LoadAttributes();
    }
    protected virtual void Awake()
    {
        LoadComponents();
        LoadAttributes();
    }
}
