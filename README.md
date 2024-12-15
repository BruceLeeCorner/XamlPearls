# RoutedEventTrigger

```xml
<StackPanel x:Name="StackPanel" Margin="0,10,0,0">
    <i:Interaction.Triggers>
        <xp:RoutedEventTrigger RoutedEvent="{x:Static ButtonBase.ClickEvent}">
            <prism:InvokeCommandAction Command="{Binding GoCommand}" TriggerParameterPath="OriginalSource.Tag" />
        </xp:RoutedEventTrigger>
    </i:Interaction.Triggers>
    <Button
        MinWidth="150"
        Margin="10"
        Padding="5"
        HorizontalAlignment="Center"
        Content="1" />
    <Button
        MinWidth="150"
        Margin="10"
        Padding="5"
        HorizontalAlignment="Center"
        Content="2" />
    <Button
        MinWidth="150"
        Margin="10"
        Padding="5"
        HorizontalAlignment="Center"
        Content="3" />
</StackPanel>
```

# Global Key Shorts

可以在Window的生命周期事件`SourceInitialized`或比其更晚的事件如`Loaded`中注册快捷键。关闭Window时(`Closed事件`)会自动注销快捷键。

```c#
// 注册快捷键
protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);

    GlobalHotkeyManager.RegisterGlobalHotKey(new HotKeyModel("hotkey1", true, false, true, false, Keys.A), (model) =>
    {
        MessageBox.Show(JsonSerializer.Serialize(model));
    }, this
    );
}
```

```c#
// 注销快捷键
GlobalHotkeyManager.UnregisterGlobalHotKey("hotkey1");
```

```c#
// 获取整个application成功注册的快捷键
IEnumerable<HotKeyModel> models = GlobalHotkeyManager.GetAllHotkeys();
```

```c#
// 获取在指定Window上注册的快捷键
IEnumerable<HotKeyModel> models = GlobalHotkeyManager.GetHotkeysOnWindow(this);
```

- 重复注册一个已经被注册过的快捷键会失败。
    - 比如Tencent QQ已经注册Ctrl + Alt +A 用于截图，如果我们开发的application再注册Ctrl + Alt + A会失败。
    - 比如我们开发的Application已经注册了Ctrl + UP，再在Application中注册Ctrl + UP会失败。
- 如果多个进程使用相同的快捷键，先启动的先占用，后来者失败。
- 一个进程只能取消本进程已经注册的快捷键，无法取消另外一个进程的快捷键。
- 快捷键事件处理程序`action`在创建window的UI线程上执行，应当妥善处理它可能抛出的异常，避免导致UI线程崩溃。
- 开发者可以随时手动注销快捷键，也可以不用关心，快捷键会在窗口关闭时自动注销





