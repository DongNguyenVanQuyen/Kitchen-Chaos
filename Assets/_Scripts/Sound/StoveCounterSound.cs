using System;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    [SerializeField] private StoveCounter soundWarning;
    private float warningSoundTimer;
    bool playWarningSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        KitchenGameManager.Instance.OnGamePaused += KitchenGameManager_OnGamePaused;
        KitchenGameManager.Instance.OnGameUnPaused += KitchenGameManager_OnGameUnPaused;
        audioSource.Stop();

        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnshowProgress = 0.5f;
        playWarningSound = e.progressNormalized >= burnshowProgress && stoveCounter.IsFried();

    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
    private void KitchenGameManager_OnGamePaused(object sender, EventArgs e)
    {
        audioSource.Pause();
    }

    private void KitchenGameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        // Chỉ play lại nếu đang trong trạng thái nấu hoặc đã chiên
        if (stoveCounter.IsFrying() || stoveCounter.IsFried())
        {
            audioSource.Play();
        }

    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f)
            {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);

            }
        }
    }
}