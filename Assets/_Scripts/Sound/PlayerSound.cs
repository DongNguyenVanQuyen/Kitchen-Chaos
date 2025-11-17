using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = 0.1f; // Thời gian giữa 2 tiếng bước chân

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // Giảm timer theo thời gian
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            // Reset lại timer
            footstepTimer = footstepTimerMax;

            if (player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volume);
            }
        }
    }
}
