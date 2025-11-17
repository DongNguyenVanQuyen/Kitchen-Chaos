using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    [SerializeField] private static Scene targetScene;

    public static void Load(Scene targerScene)
    {
        Loader.targetScene = targerScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
