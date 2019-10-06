using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class MethodStructure : UserControl
    {
        public MethodStructure()
        {
            this.InitializeComponent();
        }

        public MethodStructure(MemberInfo mInfo, bool isAccessible)
        {
            InitializeComponent();

            if (!isAccessible)
            {
                this.FindControl<Grid>("notAccessibleIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("notAccessibleIcon"), "Not accessible");
            }

            if (mInfo.MemberType == MemberTypes.Method)
            {
                MethodInfo info = (MethodInfo)mInfo;
                this.FindControl<InputTextBox>("methodNameBox").Text = info.Name;

                if (info.IsStatic)
                {
                    this.FindControl<Grid>("staticIcon").IsVisible = true;
                    ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
                }

                if (info.ReturnType != typeof(void))
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = info.ReturnType.Name;
                    ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.ReturnType.FullName);
                }
                else
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = "void";
                    this.FindControl<InputTextBox>("methodTypeBox").Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                }

                this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Margin = new Thickness(0, 0, 0, 0), Text = "(" });

                ParameterInfo[] parameters = info.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    InputTextBox bx = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = parameters[i].ParameterType.Name, FontStyle = FontStyle.Italic, Foreground = new SolidColorBrush(Color.FromRgb(0x2B, 0x91, 0xBC)), Margin = new Thickness(i == 0 ? 0 : -4, 0, 0, 0) };
                    ToolTip.SetTip(bx, parameters[i].ParameterType.FullName);
                    this.FindControl<StackPanel>("methodPanel").Children.Add(bx);
                    this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = " " + parameters[i].Name + (i < parameters.Length - 1 ? ", " : ""), Margin = new Thickness(-4, 0, 0, 0) }); ;
                }

                this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = ")", Margin = new Thickness(0, 0, 0, 0) });

            }
            else if (mInfo.MemberType == MemberTypes.Constructor)
            {
                ConstructorInfo info = (ConstructorInfo)mInfo;
                this.FindControl<InputTextBox>("methodNameBox").Text = info.Name;
                this.FindControl<InputTextBox>("methodTypeBox").Text = info.DeclaringType.Name;
                ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.DeclaringType.FullName);

                if (info.IsStatic)
                {
                    this.FindControl<Grid>("staticIcon").IsVisible = true;
                    ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
                }

                this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Margin = new Thickness(0, 0, 0, 0), Text = "(" });

                ParameterInfo[] parameters = info.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    InputTextBox bx = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = parameters[i].ParameterType.Name, FontStyle = FontStyle.Italic, Foreground = new SolidColorBrush(Color.FromRgb(0x2B, 0x91, 0xBC)), Margin = new Thickness(i == 0 ? 0 : -4, 0, 0, 0) };
                    ToolTip.SetTip(bx, parameters[i].ParameterType.FullName);
                    this.FindControl<StackPanel>("methodPanel").Children.Add(bx);
                    this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = " " + parameters[i].Name + (i < parameters.Length - 1 ? ", " : ""), Margin = new Thickness(-4, 0, 0, 0) });
                }

                this.FindControl<StackPanel>("methodPanel").Children.Add(new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.5, 0, 0), MyReadOnly = true, Text = ")", Margin = new Thickness(0, 0, 0, 0) });
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
