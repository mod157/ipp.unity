# InputPaternPanel for Unity
[![Releases](https://img.shields.io/github/release/mod157/ipp.unity.svg)](https://github.com/mod157/ipp.unity/releases) 
<!--[![Readme_EN](https://img.shields.io/badge/ipp.unity-en-red)](https://github.com/mod157/ipp.unity/README_EN.md)-->


InputPaternPanel은 패턴 이벤트를 간편하게 처리할 수 있도록 도와주는 라이브러리입니다.
* 입력 패턴에 따라 적절한 이벤트를 처리할 수 있습니다.
* 사용자의 입력을 효과적으로 감지하고 이에 따라 프로그램 또는 애플리케이션의 동작을 조절하는 데 유용합니다.
* 간단한 코드로 다양한 입력 패턴을 처리할 수 있어 많은 편의를 제공합니다.
---
Nammu - DreamAntDev
Email : nammu8395@gmail.com
GitHub Issue : https://github.com/mod157/ipp.unity/issues

---
## Contents
- [Getting started](#getting-started)
- [UPM package](#upm-package)


History
---
v1.0.0 - 패키지 추가

Getting started
---
asset package(`ipp.unity.*.*.*.unitypackage`) available in [ipp.unity/releases](https://github.com/mod157/ipp.unity/releases) page.

| .NET Type | UniTask Type | 
| --- | --- |
| `IProgress<T>` | --- |
| `CancellationToken` | --- | 
| `CancellationTokenSource` | --- |

주요 기능
---
Pattern 입력을 통한 결과값 전달

```csharp
public class ZeroAllocAsyncAwaitInDotNetCore
{
    public ValueTask<int> DoAsync(int x, int y)
    {
        return Core(this, x, y);

        static async UniTask<int> Core(ZeroAllocAsyncAwaitInDotNetCore self, int x, int y)
        {
            // do anything...
            await Task.Delay(TimeSpan.FromSeconds(x + y));
            await UniTask.Yield();

            return 10;
        }
    }
}
```

License
---
This library is under the [MIT](https://github.com/mod157/ipp.unity?tab=MIT-1-ov-file) License.
