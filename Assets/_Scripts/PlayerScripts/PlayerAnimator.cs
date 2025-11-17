using UnityEngine;

public class PlayerAnimator : AntBehaviour
{
    [SerializeField] private Animator _animator;
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player _player;

    protected override void LoadComponents() 
    {
        base.LoadComponents();
        LoadAnimator();
        LoadPlayer();
    }

    void LoadAnimator()
    {
        if (_animator != null) return;
        _animator = GetComponent<Animator>();
    }

    void LoadPlayer()
    {
        if (_player != null) return;
        _player = transform.parent.GetComponent<Player>();
    }

    protected override void LoadAttributes()
    {
        base.LoadAttributes();
    }

    private void Update()
    {
        _animator.SetBool(IS_WALKING, _player.IsWalking());
    }
}
