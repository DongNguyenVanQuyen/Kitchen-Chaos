using System;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickedSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }

    [SerializeField] private LayerMask _countersLayerMask;
    [SerializeField] private BaseCounter _selectedCounter;
    [SerializeField] private bool _isWalking = false;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float heightOffset;

    private Vector3 moveDir;
    private Vector3 lastMoveDir;
    private Vector3 lastInteractDir;
    private float playerRadius = 0.7f;

    [SerializeField] private KitchenObjects _kitchenObject;
    [SerializeField] private Transform _kitchenObjectHoldPoint;

    void LoadCounterLayerMask()
    {
        if (_countersLayerMask != 0) return;
        _countersLayerMask = LayerMask.GetMask("Counters");
    }

    private void Awake()
    {
        LoadCounterLayerMask();
    //    Player.Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this); // Gọi tương tác thay thế chỉ 1 lần
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this); // Gọi tương tác chỉ 1 lần
        }
    }

    private void Update()
    {
        // Chỉ được di chuyển và tương tác nếu là chủ
        if (!IsOwner){
            return;
        }
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        Vector3 dir = moveDir != Vector3.zero ? moveDir : lastInteractDir;
        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, dir, out RaycastHit raycastHit, interactDistance, _countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != _selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                    Debug.Log("Selected Counter: " + baseCounter.name);
                }
            }
            else
            {
                if (_selectedCounter != null)
                {
                    SetSelectedCounter(null);
                }
            }
        }
        else
        {
            if (_selectedCounter != null)
            {
                SetSelectedCounter(null);
            }
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this._selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            SelectedCounter = _selectedCounter
        });
    }
    private void HandleMovement()
    {
        Vector2 inputMovement = GameInput.Instance.GetMovementVector();
        moveDir = new Vector3(inputMovement.x, 0, inputMovement.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerHeight = 2f;

        if (moveDir != Vector3.zero)
        {
            lastMoveDir = moveDir;
        }

        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance
        );

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -0.5f || moveDir.x > +0.5f) && !Physics.CapsuleCast(
                transform.position,
                transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance
            );
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > +0.5f) && !Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance
                );
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        _isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, lastMoveDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return _isWalking;
    }



    // IKitchenObjectParent implementation
    public Transform GetKitchenObjectFollowTransform() => _kitchenObjectHoldPoint;

    public void SetKitchenObject(KitchenObjects kitchenObject)
    {
        this._kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObjects GetKitchenObject() => _kitchenObject;

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() => _kitchenObject != null;

    public NetworkObject GetNetworkObject() => NetworkObject;




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;

        Vector3 point1 = transform.position + Vector3.up * 0.1f;
        Vector3 point2 = transform.position + Vector3.up * (playerHeight - 0.1f);

        Vector3 direction = moveDir.normalized;
        Vector3 offset = direction * moveDistance;

        Gizmos.DrawWireSphere(point1, playerRadius);
        Gizmos.DrawWireSphere(point2, playerRadius);
        Gizmos.DrawLine(point1 + Vector3.right * playerRadius, point2 + Vector3.right * playerRadius);
        Gizmos.DrawLine(point1 - Vector3.right * playerRadius, point2 - Vector3.right * playerRadius);
        Gizmos.DrawLine(point1 + Vector3.forward * playerRadius, point2 + Vector3.forward * playerRadius);
        Gizmos.DrawLine(point1 - Vector3.forward * playerRadius, point2 - Vector3.forward * playerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(point1, point1 + offset);
        Gizmos.DrawLine(point2, point2 + offset);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(point1 + offset, playerRadius);
        Gizmos.DrawWireSphere(point2 + offset, playerRadius);
    }
}
