# Unity Singleton
可以在Editor和Runtime混用的Unity单例模块。
## Features
* 支持Editor和Runtime
* 稳定可靠，不会多实例化
* 懒加载
* 兼容不同的`Reload Domain`选项
* 兼容几乎所有情况，可靠的提供有且只有一个的单例
* 配备单例列表可视化窗口（单例默认使用HideFlags隐藏）
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