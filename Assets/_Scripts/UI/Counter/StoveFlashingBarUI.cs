using UnityEngine;

public class StoveFlashingBarUI : MonoBehaviour
{
    private const string IS_FLashing = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;


    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool(IS_FLashing, false);

        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnshowProgress = 0.5f;
        bool show = e.progressNormalized >= burnshowProgress && stoveCounter.IsFried();

       animator.SetBool(IS_FLashing, show);

    }

  
}
