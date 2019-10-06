using Avalonia;
using Avalonia.Markup.Xaml;

namespace ScriptConsole
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
