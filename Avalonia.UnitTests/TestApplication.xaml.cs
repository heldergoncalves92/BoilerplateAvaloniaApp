using Avalonia;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;

[assembly: AvaloniaTestApplication(typeof(TestApplicationBuilder))]
public class TestApplicationBuilder {
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApplication>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public class TestApplication : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }
}