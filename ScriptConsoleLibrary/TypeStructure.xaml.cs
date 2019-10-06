using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class TypeStructure : UserControl
    {
        public TypeStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public TypeStructure(TypeInfo info, bool isAccessible)
        {
            InitializeComponent();

            if (!isAccessible)
            {
                this.FindControl<Grid>("notAccessibleIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("notAccessibleIcon"), "Not accessible");
            }

            if (info.IsAbstract && info.IsSealed)
            {
                this.FindControl<Grid>("staticIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
            }

            this.FindControl<InputTextBox>("methodTypeBox").Text = info.Name;
            ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.FullName);

            if (typeof(Delegate).IsAssignableFrom(info.BaseType))
            {
                this.FindControl<Grid>("delegateIcon").IsVisible = true;
            }
            else if (info.IsClass)
            {
                this.FindControl<Grid>("classIcon").IsVisible = true;
            }
            else if (info.IsInterface)
            {
                this.FindControl<Grid>("interfaceIcon").IsVisible = true;
            }
            else if (info.IsEnum)
            {
                this.FindControl<Grid>("enumIcon").IsVisible = true;
            }
            else if (info.IsValueType)
            {
                this.FindControl<Grid>("structIcon").IsVisible = true;
            }
        }
    }
}
