using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter _cutitngCounter;

    [SerializeField] private Animator _animator;

    private const string CUT = "Cut";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _cutitngCounter.OnCut += CutitngCounter_OnCut;
    }
    private void CutitngCounter_OnCut(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(CUT);
    }

}
