# Sound Manager

## 필요한 기능

- BGM Fade In / Fade Out
- 동시에 사운드 재생
- asset sound를 교체할 수 있는 구조. Reference를 코드가 아니고 weak reference를 이용한다.

## Fade In / Fade Out

- 2개의 Audio Source 필요
- Fade In / Out를 적용하기 위한 update loop 혹은 코루틴이 필요
- Fading 도중 새로운 Fade 들어왔을 경우의 정책 필요

### Fading

``` Csharp
struct FadeData
{
    float fadeTimer;
    float fadeDuration;
    float startVolume;
    float targetVolum;
    Interpolatetype type;
}
```

Fade를 진행하기 위해서는 Fade 기록을 관리해야 한다.

## Audio 재생 정책

### Play BGM

- 현재 재생 중인 BGM이 없을 경우는 비어 있는 Audio Source에서 재생
- 현재 재생 중인 BGM이 있을 경우는 재생 중인 BGM을 정지하고 새로운 BGM 재생
- Fading 중에는 Fade를 정지하고 새로운 BGM을 재생

### Fade In

- 현재 재생 중인 BGM이 없을 경우는 비어 있는 Audio SOurce에서 Fade in 진행
- 현재 재생 중인 BGM이 있을 경우는 재생 중인 BGM을 Fade Out, 새로운 Audio Source에서 Fade In을 진행
- Fading 중에는 가급적이면 Fade를 실행하지 않는다.
- Fading 중을 확인할 수 있는 변수 제공
- Fading 중에 어떠한 Audio Source를 정지할지에 대해서 선택지 제공

## Resource

- Sound 에 대한 정보 : Sound Asset Path, Sound Name(혹은 Addressable로 해결 가능)
- Fade에 대한 정보
