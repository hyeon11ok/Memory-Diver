**범위 내 사물 외곽선 렌더링 쉐이더**

**세팅 방법**
- URP 기준
- Assets/Settings/PC_Renderer 선택
- Full Screen Pass Renderer Feature 추가
- Pass Material에 Outlines 등록
- Injection Point를 Before Rendering Post Processing으로 변경

**Material 변수**
- OutlineThickness : 외곽선 두께
- OutlineColor : 외곽선 색상
- ScoutCenter : 렌더링 기준점 위치
- ScoutRadius : 외곽선 렌더링 범위
- Softness : 렌더링 경계면 흐려지는 범위