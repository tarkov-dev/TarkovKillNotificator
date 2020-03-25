# TarkovKillNotificator
Nvidia Highlights에서 저장하는 임시 파일을 탐지하여 타르코프 게임 내 킬을 알려주는 프로그램입니다.

<p align="center">
  <img src="https://user-images.githubusercontent.com/62662342/77567930-33a50e80-6f0b-11ea-9b4f-c447622d3cf3.png">
</p>

[다운로드](https://github.com/tarkov-dev/TarkovKillNotificator/releases)

## 기능 설명

- 임시 폴더 선택 기능
- 사용자 정의 사운드 재생 기능
- 킬 카운트 출력 기능
- 팝업 기능 (하단 이미지 참고)
<p align="center">
  <img src="https://user-images.githubusercontent.com/62662342/77567938-36076880-6f0b-11ea-811e-a9455e265b49.png">
</p>

## 기본 사용법

프로그램을 다운로드하거나, Git을 Clone하여 직접 Build한 후 생성되는 TarkovKillNotificator.exe 파일을 실행해주세요.

이후 기본 임시 폴더를 사용하시거나 Geforce experience - 설정 - 하이라이트 - 스토리지 (임시 파일) 에 지정된 폴더를 하이라이트 폴더로 지정해주세요.

하이라이트 폴더는 기본값처럼 Highlight\Escape From Tarkov 형식으로 되어 있습니다.

하이라이트 폴더를 정상적으로 지정하였다면 모니터링 시작 버튼을 누르면 샘플 사운드가 재생되며 모니터링이 시작됩니다. 이후 게임에서 킬을 하게 되면 소리를 들을 수 있습니다.

## 팝업 사용법

팝업 기능을 이용하시려면 게임을 창 모드나 테두리 없음 모드로 설정해야 합니다.

프로그램에서 팝업 열기 버튼을 클릭하시면 빨간색 글자로 된 Kill: 0 창이 나타납니다. 텍스트에 정확히 마우스를 올리고 드래그하면 창의 위치를 이동할 수 있습니다.

팝업 창은 기본적으로 항상 위 설정이 되어 있으므로 창 모드나 테두리 없음 모드인 게임 위에 표시될 수 있습니다. 마우스 클릭이 가능하므로 가능한 한 가장자리에 배치해 주세요.

## 버전 일람

### 1.0.0 (2020.03.26)
- 첫 릴리즈

## 오픈소스 라이브러리

[MahApps.Metro](https://github.com/MahApps/MahApps.Metro)<br>
[ini-parser](https://github.com/rickyah/ini-parser)

## 라이선스

[MIT License (MIT)](LICENSE)
