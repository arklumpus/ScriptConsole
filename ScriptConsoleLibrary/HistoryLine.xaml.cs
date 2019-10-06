using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ScriptConsoleLibrary
{
    internal class HistoryLine : UserControl
    {
        public HistoryLine()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public HistoryLine(string command)
        {
            InitializeComponent();
            this.FindControl<InputTextBox>("commandBox").Text = command;
        }


    }
}
