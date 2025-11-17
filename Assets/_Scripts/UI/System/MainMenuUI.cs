using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playbutton;
    [SerializeField] private Button quitbutton;

    private void Awake()
    {
        playbutton.onClick.AddListener(() =>
        {
           Loader.Load(Loader.Scene.GameScene);
        });
        quitbutton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
   
}
