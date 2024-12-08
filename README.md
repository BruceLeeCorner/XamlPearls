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



- 不管window和id是什么，重复注册一个已经被注册过的快捷键会失败。
    - 比如Tencent QQ已经注册Ctrl + Alt +A 用于截图，如果我们开发的application再注册Ctrl + Alt + A会失败。
    - 比如我们开发的Application已经注册了Ctrl + UP，再在Application中注册Ctrl + UP会失败。

- 如果多个进程使用相同的快捷键，先启动的先占用，后来者失败。
- 一个进程无法取消另外一个进程的快捷键然后自己再占用，因为一个进程无法获得另一个进程注册该快捷键使用的id和window。
- 进程用字典(key: window+id，value: hotkey)存储注册信息，id可以是0，1，2，3 ...... 各个进程的id互相独立，允许重复.比如微信可以用123，QQ也能用123。
- 进程如果使用重复的window+id注册，仍旧会注册成功，后者会覆盖掉前者。
- 取消注册时使用window+id,而不是用快捷键。
- 关闭窗口时，尽量保证显示取消该窗口注册的快捷键。
- action在窗口所在的UI线程执行。
- 退出程序时，可以注销所有快捷键，也可以不注销。但是如果仅在一个副窗口存在时，那么需要手动注销。





