![UnitySingleton](https://socialify.git.ci/labbbirder/UnitySingleton/image?description=1&font=Jost&forks=1&issues=1&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB2aWV3Qm94PSIwIDAgNTEyIDUxMiI%2BPHBhdGggZD0iTTIzOS4xIDYuM2wtMjA4IDc4Yy0xOC43IDctMzEuMSAyNS0zMS4xIDQ1djIyNS4xYzAgMTguMiAxMC4zIDM0LjggMjYuNSA0Mi45bDIwOCAxMDRjMTMuNSA2LjggMjkuNCA2LjggNDIuOSAwbDIwOC0xMDRjMTYuMy04LjEgMjYuNS0yNC44IDI2LjUtNDIuOVYxMjkuM2MwLTIwLTEyLjQtMzcuOS0zMS4xLTQ0LjlsLTIwOC03OEMyNjIgMi4yIDI1MCAyLjIgMjM5LjEgNi4zek0yNTYgNjguNGwxOTIgNzJ2MS4xbC0xOTIgNzhsLTE5Mi03OHYtMS4xbDE5Mi03MnptMzIgMzU2VjI3NS41bDE2MC02NXYxMzMuOWwtMTYwIDgweiIgZmlsbD0iY3VycmVudENvbG9yIj48L3BhdGg%2BPC9zdmc%2B&name=1&stargazers=1&theme=Auto)
可以在Editor和Runtime混用的Unity单例模块。稳定可靠，懒加载，无额外开销，任何情况都不会出现多实例化。
## Features
* 兼容所有场景，稳定可靠的提供一个单例：
    * Editor和Runtime混合使用
    * 中途重新编译脚本
    * 不同的`Reload Domain`选项
    * 场景切换
    * etc
* 可以包含子物体：基于MonoBehaviour，可添加子GameObject
* 独立于场景：单例默认使用HideFlags隐藏，不属于任何场景，不需要考虑入口场景
* 可检索：可视化窗口检索单例列表（Window/bbbirder/Singletons）
## Example
只需要继承`Singleton<T>`
```csharp
using UnityEngine;
using com.bbbirder.unity;
public class YourComponent:Singleton<YourComponent>{
    protected override void Awake(){
        SayHello();
    }
    public void SayHello(){
        print("hello,world");
    }
}
```
需要调用的地方 ：
```csharp
    YourComponent.Instance.SayHello();
```
通过Window/bbbirder/Singletons窗口查看当前的单例

![窗口截图](Documentation/img_record.png)
## Known Issues
1. 可视化窗口在部分低版本Unity中不自动刷新，需要手动改变下窗口大小解决。或者升级Unity版本
