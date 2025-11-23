using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransfrom;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransfrom = targetTransform;
    }
    private void LateUpdate()
    {
        if (targetTransfrom != null)
        {
            transform.position = targetTransfrom.position;
            transform.rotation = targetTransfrom.rotation;
        }
    }
}

