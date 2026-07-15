using UnityEngine;

[System.Serializable]
public class SceneField : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    // 에디터 인스펙터 창에서만 보이는 드래그 앤 드롭용 씬 에셋 칸
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    // 실제 빌드에 포함되어 런타임에 쓰일 씬 이름 (인스펙터에서는 숨김)
    [SerializeField, HideInInspector]
    private string sceneName = "";

    // 외부에서 씬 이름을 읽을 때 사용하는 프로퍼티
    public string SceneName => sceneName;

    // 편의 기능: SceneField 객체 자체를 string처럼 바로 쓸 수 있게 해주는 마법
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.SceneName;
    }

    // ==============================================================
    // 유니티가 인스펙터의 변경 사항을 저장(직렬화)할 때마다 자동으로 실행됨
    // ==============================================================
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        // 개발자가 인스펙터에 씬 에셋을 넣으면, 그 에셋의 '이름'만 몰래 string에 복사해 둡니다!
        if(sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
        else
        {
            sceneName = "";
        }
#endif
    }

    public void OnAfterDeserialize() { }
}
