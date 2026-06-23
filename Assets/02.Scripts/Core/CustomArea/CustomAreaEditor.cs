using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// CustomArea 컴포넌트의 에디터 화면을 커스텀하겠다고 선언합니다.
[CustomEditor(typeof(CustomArea))]
public class CustomAreaEditor:Editor
{
    // 박스 형태의 핸들을 쉽게 그려주는 클래스입니다.
    private BoxBoundsHandle boxHandle = new BoxBoundsHandle();

    // 씬 뷰에 무언가를 그릴 때 호출되는 메서드입니다.
    protected virtual void OnSceneGUI()
    {
        CustomArea area = (CustomArea)target;

        // 핸들의 색상을 지정합니다 (콜라이더처럼 초록색으로 설정)
        Handles.color = Color.green;

        // 게임 오브젝트의 회전(Rotation)과 스케일(Scale)이 핸들에도 적용되도록 매트릭스를 설정합니다.
        Matrix4x4 handleMatrix = area.transform.localToWorldMatrix;
        Handles.matrix = handleMatrix;

        // 핸들의 현재 크기와 위치를 스크립트의 데이터와 동기화합니다.
        boxHandle.center = area.center;
        boxHandle.size = area.size;

        // 값의 변경이 있었는지 체크를 시작합니다.
        EditorGUI.BeginChangeCheck();

        // 씬 뷰에 박스 핸들을 그립니다.
        boxHandle.DrawHandle();

        // 마우스로 핸들을 드래그해서 값이 변경되었다면
        if(EditorGUI.EndChangeCheck())
        {
            // Ctrl+Z (실행 취소)를 위해 변경 사항을 기록합니다.
            Undo.RecordObject(area, "Change Custom Area Bounds");

            // 드래그해서 변한 핸들의 값을 다시 컴포넌트에 덮어씌웁니다.
            area.center = boxHandle.center;
            area.size = boxHandle.size;
        }
    }
}