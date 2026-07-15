using UnityEngine;

public class SceneDatabase : Singleton<SceneDatabase>
{
    // SceneAsset이나 string 대신, 방금 만든 SceneField를 사용합니다.
    [SerializeField] private SceneField titleScene;
    [SerializeField] private SceneField gameScene;

    // 외부에서는 기존처럼 string으로 꺼내서 쓰면 됩니다!
    public string TitleScene => titleScene.SceneName;
    public string GameScene => gameScene.SceneName;
}
