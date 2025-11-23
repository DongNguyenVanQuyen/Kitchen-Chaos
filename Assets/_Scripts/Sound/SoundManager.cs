using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SOUND_VOLUME = "SoundVolume";

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private float volume = 1f;

    private void Awake()
    {
        Instance = this;

        // Load volume từ PlayerPrefs
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 1f);
    }
    private void Start()
    {
        DeliverManager.Instance.OnRecipeSuccess += Delivery_OnRecipeSuccess;
        DeliverManager.Instance.OnRecipeFailed += Delivery_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnAnyPlayerPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
        }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter  baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void Delivery_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    }

    private void Delivery_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);

    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumn = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumn);

    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumn = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumn * this.volume);
    }

    public void PlayFootstepsSound(Vector3 position, float volumn)
    {
        PlaySound(audioClipRefsSO.footstep, position, volumn);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.warning, position);
    }

    public void ChangeVolume()
    {
        // Implementation for changing volume
        volume += 0.1f;
        volume = volume % 1.1f;
        if (volume < 0.1f)
        {
            volume = 0f;
        }
        // Luu volume xuong PlayerPrefs
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
