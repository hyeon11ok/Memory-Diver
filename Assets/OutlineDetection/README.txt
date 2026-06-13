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

**트러블 슈팅**
- 연산 횟수로 인한 성능 저하 문제
  - 기존의 셰이더는 화면 전체의 픽셀 마다 깊이와 노멀 각각 8방향씩 체크
  - 연산 수를 줄이기 위해 깊이와 노멀 체크를 4방향으로 줄임
  - 또한, 깊이와 노멀	체크를 한 번에 처리하여 연산 횟수 감소

- 특정 오브젝트는 종류별로 다른 색상의 외곽선이 필요
  - 셰이더에서 오브젝트의 색상을 기반으로 외곽선 색상 결정
  - 오브젝트의 색상에 따라 외곽선 색상이 달라짐

- 두 개 이상의 오브젝트가 겹쳐질 때 오브젝트가 겹치는 부분의 외곽선이 중복으로 그려져 지저분해보임
  - 주변 픽셀과의 깊이 차이를 누적할 때 주변 픽셀이 현재 픽셀보다 멀리 있는 경우에만 누적하여 더 앞에 있는 픽셀만 그려지도록 함
  - 노멀 값의 차이도 깊이 값의 차이와 같은 방식으로 누적하여 더 앞에 있는 픽셀만 그려지도록 함