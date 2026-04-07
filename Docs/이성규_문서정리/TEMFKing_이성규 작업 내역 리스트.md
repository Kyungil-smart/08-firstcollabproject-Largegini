이성규 작업 내역 리스트

---

**보드 시스템**
- 12×5 그리드 (버퍼 6행 + 플레이 6행), RectMask2D 마스킹
- 블록 오브젝트 리사이클 (파괴 없이 Despawn→Init 재사용)
- 초기 3매치 방지 / 시드 기반 보드 생성

**입력**
- 드래그 앤 드롭 스왑 (UI좌표↔그리드 인덱스 역변환, 이중 클램핑)
- 연출 중 입력 차단 / 외부 상호작용 제어(SetInteractable)

**매칭·연쇄 루프**
- 가로/세로 3+ 매치 탐색 (L·T자 자동 포함)
- 스왑→매칭→제거→낙하→리필→재매칭 자동 연쇄
- 이펙트 완료 콜백 + clearDelay 이중 대기로 타이밍 버그 방지

**결과 출력**
- PuzzleResult (타입별 블록 수 + 콤보) UnityEvent 전달
- 데드락 판정 → 이벤트 발사

**연출**
- DOTween 스왑/낙하 애니메이션, 블록 매치 프레임 이펙트
- 콤보 UI (팝→상승 페이드아웃) + SFX 3종 분기

**튜토리얼 훅**
- ITutorialBoardControl — 입력 잠금, 좌표 필터, 방향 필터, 체인 인터셉터, 프리셋 보드

**아키텍처**
- MonoBehaviour 3개 + 순수 C# 하위 시스템 5개
- 인터페이스 4종으로 의존성 분리
- PuzzleComponentsBinder로 이벤트↔UI·SFX 느슨한 결합

---

**세부 내역**


**보드 기초**

- Grid2D 자료구조 — 1D 배열 + 인덱서로 직렬화 가능한 2D 그리드
- 보드 레이아웃 — 12×5 (버퍼 6행 + 플레이 6행), RectMask2D로 버퍼 마스킹
- 블록 재사용 — 파괴 없이 Despawn→재배치→Init으로 오브젝트 리사이클
- 초기 3매치 방지 — 좌측/상단 2칸 체크 후 매치 유발 타입 필터링하여 스폰
- 시드 기반 초기화 — System.Random으로 결정적/비결정적 보드 생성 분기

**입력 시스템**

- 드래그 앤 드롭 스왑 — IPointerDown/Drag/Up + UI좌표→그리드 인덱스 역변환
- 드래그 클램핑 — stride+셀사이즈 범위 + 보드 외곽 이중 제한
- 셀 하이라이트 — 드래그 중 호버 셀에 인접/대각선 판별 색상 표시
- 보드 프로세싱 잠금 — \_isProcessing 플래그로 연출 중 전체 입력 차단
- SetInteractable — GraphicRaycaster on/off로 외부에서 보드 입력 제어

**매칭·연쇄**

- 매치 탐색 — 가로/세로 독립 스캔, 3+ 연속 동일타입 판정 (L/T자 자동 포함)
- 연쇄 루프 — 스왑→매칭→제거→낙하→리필→재매칭 반복, 없으면 종료
- 클리어 — HashSet으로 교차점 중복 제거 후 블록 Despawn
- 낙하 — 열별 빈칸 아래부터 탐색, 활성 블록 당겨내림 + DOAnchorPos 연출
- 리필 — Despawn된 블록을 리사이클 풀에서 꺼내 버퍼 상단에 Init 재배치
- 비동기 타이밍 안전장치 — clearDelay + 이펙트 완료 콜백 이중 대기로 고스트 블록 방지

**결과 출력**

- PuzzleResult — 타입별 매치 블록 수 + 콤보 카운트, UnityEvent로 전투에 전달
- 데드락 판정 — 플레이 영역 전체 인접 스왑 시뮬레이션, 불가 시 UnityEvent 발사

**연출·VFX·SFX**

- 스왑 DoTween — DOAnchorPos + Moving 상태 전환으로 연출 중 입력 차단
- 블록 매치 이펙트 — UIFrameEffect로 스프라이트 프레임 순차 재생, 완료 후 Despawn
- 콤보 UI — TMP + DOTween Sequence (팝 스케일→유지→상승 페이드아웃)
- 퍼즐 SFX — PlayOneShot 중첩 재생, 스왑/저콤보/고콤보 3종 분기

**튜토리얼 훅**

- ITutorialBoardControl — 입력 잠금, 좌표 화이트리스트, 스왑 방향 필터, 체인 인터셉터, 프리셋 보드 로드, 탭 감지
- 하이라이트 2레이어 분리 — 드래그용(블록 자식) / 튜토리얼용(보드 자식) 독립 관리

**인프라·설정**

- 아키텍처 — MonoBehaviour 3개 + 순수 C# 하위 시스템 (Spawner, Swapper, Processor, Validator, TutorialHandler)
- 인터페이스 분리 — IBoardInteraction / IBoardQuery / IBoardData / ITutorialBoardControl
- BoardAnimationSettings — 직렬화 클래스로 연출 수치 인스펙터 튜닝
- PuzzleComponentsBinder — 보드 이벤트↔UI·SFX 느슨한 결합 중계
- 사운드 슬라이더 — Addressable AudioMixer + MixerVolumeController + VolumeSlider 바인딩
- 스프라이트 아틀라스 — 퍼즐/스킬아이콘별 아틀라스 패킹, 회전 해제, 이중 압축 방지

---