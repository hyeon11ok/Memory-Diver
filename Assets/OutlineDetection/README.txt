**범위 내 사물 외곽선 렌더링 쉐이더**

**세팅 방법**
- URP 기준
- Assets/Settings/PC_Renderer 선택
- Full Screen Pass Renderer Feature 추가
- Pass Material에 Outlines 등록
- Injection Point를 Before Rendering Post Processing으로 변경
- 플레이어 오브젝트에 PlayerPositionProvider 스크립트 추가
- Outline 색상은 오브젝트의 색상에 따라 달라짐

**Material 변수**
- OutlineThickness : 외곽선 두께
- DetectionCenter : 렌더링 기준점 위치
- DetectionRadius : 외곽선 렌더링 범위
- Softness : 렌더링 경계면 흐려지는 범위

**PlayerPositionProvider**
- DetectionCenter에 플레이어 위치를 전달하는 스크립트
- DetectionRadius, Softness 변수 조절 가능
- 외부에서 DetectionRadius 값 조절 가능