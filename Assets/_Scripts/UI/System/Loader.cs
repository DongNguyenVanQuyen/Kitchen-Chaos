using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene,
    }

    [SerializeField] private static Scene targetScene;

    public static void Load(Scene targerScene)
    {
        Loader.targetScene = targerScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoadNetwork(Scene targerScene)
    {
       NetworkManager.Singleton.SceneManager.LoadScene(targerScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
