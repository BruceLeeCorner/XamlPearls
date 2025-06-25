# Global Key Shorts

可以在Window的生命周期事件`SourceInitialized`或比其更晚的事件如`Loaded`中注册快捷键。关闭Window时(`Closed事件`)会自动注销快捷键。

```c#
// 注册快捷键
protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);
    this.GlobalHotkeyManager.RegisterGlobalHotKey(new HotKeyModel("hotkey1", true, false, true, false, Keys.A), (model) =>
    {
        MessageBox.Show(JsonSerializer.Serialize(model));
    });
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

- 快捷键事件处理程序`action`在创建window的UI 线程上执行，应当妥善处理它可能抛出的异常，避免导致UI线程崩溃。
- 其他进程或当前进程已经注册的快捷键，重复注册会失败。
- 如果多个进程使用相同的快捷键，先启动的先占用，后来者失败。
- 一个进程只能注销本进程已经注册的快捷键，无法注销另外一个进程的快捷键。
- 开发者可以随时注销已经注册的快捷键，也可以不用关心，`快捷键会在窗口关闭时自动注销`。
- 一般是在主窗体的OnSourceInitialized注册快捷键，只要主窗体没被关闭，快捷键一直有效。

# BindingProxy

UserControl有依赖属性`IsDescriptionColumnVisible`,子控件`DataGrid`.

```c#
public bool IsDescriptionColumnVisible
{
    get { return (bool)GetValue(IsDescriptionColumnVisibleProperty); }
    set { SetValue(IsDescriptionColumnVisibleProperty, value); }
}
```

DataGrid的`Description Column`的可见性绑定到依赖属性`IsDescriptionColumnVisible`,

如，`Visibility="{Binding ElementName="userControl", Path=IsDescriptionColumnVisible, Converter={StaticResource Bool2VisibilityConverter}}"`

但是会发现并不起作用。打开Visual Studio的Output窗口可以看到错误信息`System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=IsDescriptionColumnVisible; DataItem=null; target element is 'DataGridTemplateColumn' (HashCode=53540541); target property is 'Visibility' (type 'Visibility')`

总之就是DataGridTemplateColumn不在UserControl的可视化树上，导致绑定无效。

使用BindingProxy可以解决问题。在资源中定义BindingProxy，BindingProxy的DataContext与资源宿主的DataContext相同。

```xml
<UserControl.Resources>
    <xp:BindingProxy x:Key="proxy" Data="{Binding ElementName=self}" />
</UserControl.Resources>

<DataGridTemplateColumn
    Header="Description"
    MinWidth="{Binding Source={StaticResource proxy}, Path=Data.DescColumnMinWidth}"
    MaxWidth="{Binding Source={StaticResource proxy}, Path=Data.DescColumnMaxWidth}"
    Visibility="{Binding Source={StaticResource proxy}, Path=Data.IsDescriptionColumnVisible, Converter={StaticResource Bool2VisibilityConverter}}">
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Description}" />
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
</DataGridTemplateColumn>

```
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



