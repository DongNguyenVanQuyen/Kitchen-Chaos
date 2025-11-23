using System;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter _selectedCounter;
    [SerializeField] private GameObject[] _visualGameObjectArray;
    private void Start()
    {
        if (Player.LocalInstance != null)
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        else
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
    }

    private void Player_OnAnyPlayerSpawned(object sender, EventArgs e)
    {

        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }

    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.SelectedCounter == _selectedCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        foreach (var _visualGameObject in _visualGameObjectArray)
                _visualGameObject.SetActive(true);
    }
    private void Hide()
    {
        foreach (var _visualGameObject in _visualGameObjectArray)
                _visualGameObject.SetActive(false);
    }
}

