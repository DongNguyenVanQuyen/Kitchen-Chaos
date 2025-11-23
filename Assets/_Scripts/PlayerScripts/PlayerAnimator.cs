using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player _player;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }
    }
    private void Update()
    {
        if (!IsOwner) return;
        _animator.SetBool(IS_WALKING, _player.IsWalking());
    }
}
