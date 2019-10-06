using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class UnknownStructure : UserControl
    {
        public UnknownStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public UnknownStructure(MemberInfo mInfo)
        {
            InitializeComponent();

            this.FindControl<Grid>("propertyIcon").IsVisible = true;

            this.FindControl<InputTextBox>("methodNameBox").Text = mInfo.Name;
        }
    }
}
