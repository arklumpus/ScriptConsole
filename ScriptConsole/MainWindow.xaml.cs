using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;

namespace ScriptConsole
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Icon = new WindowIcon(new MemoryStream(CSharpIcon));
            this.Closed += (s, e) =>
            {
                this.FindControl<ScriptConsoleLibrary.ScriptConsoleControl>("scc").CleanUp();
            };
        }


    }
}
