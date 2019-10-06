using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class EventStructure : UserControl
    {
        public EventStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public EventStructure(EventInfo info, bool isAccessible)
        {
            InitializeComponent();

            if (!isAccessible)
            {
                this.FindControl<Grid>("notAccessibleIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("notAccessibleIcon"), "Not accessible");
            }

            if (info.AddMethod != null && info.AddMethod.IsStatic)
            {
                this.FindControl<Grid>("staticIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
            }

            this.FindControl<InputTextBox>("methodTypeBox").Text = info.EventHandlerType.Name;
            ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.EventHandlerType.FullName);

            this.FindControl<InputTextBox>("methodNameBox").Text = info.Name;
        }
    }
}
