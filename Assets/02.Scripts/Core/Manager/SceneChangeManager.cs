using UnityEditor;
using UnityEngine;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    [SerializeField] private SceneAsset titleScene;
    [SerializeField] private SceneAsset gameScene;

    public SceneAsset TitleScene => titleScene;
    public SceneAsset GameScene => gameScene;
}
