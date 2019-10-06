using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Reflection;

namespace ScriptConsoleLibrary
{
    internal class PropertyStructure : UserControl
    {
        public PropertyStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public PropertyStructure(MemberInfo mInfo, bool isAccessible)
        {
            InitializeComponent();

            if (!isAccessible)
            {
                this.FindControl<Grid>("notAccessibleIcon").IsVisible = true;
                ToolTip.SetTip(this.FindControl<Grid>("notAccessibleIcon"), "Not accessible");
            }

            if (mInfo.MemberType == MemberTypes.Property)
            {
                this.FindControl<Grid>("propertyIcon").IsVisible = true;

                PropertyInfo info = (PropertyInfo)mInfo;
                this.FindControl<InputTextBox>("methodNameBox").Text = info.Name;

                if ((info.GetMethod != null && info.GetMethod.IsStatic) || (info.SetMethod != null && info.SetMethod.IsStatic))
                {
                    this.FindControl<Grid>("staticIcon").IsVisible = true;
                    ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
                }

                if (info.PropertyType != typeof(void))
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = info.PropertyType.Name;
                    ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.PropertyType.FullName);
                }
                else
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = "void";
                    this.FindControl<InputTextBox>("methodTypeBox").Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                }
            }
            else if (mInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo info = (FieldInfo)mInfo;

                if (info.IsStatic)
                {
                    this.FindControl<Grid>("staticIcon").IsVisible = true;
                    ToolTip.SetTip(this.FindControl<Grid>("staticIcon"), "Static");
                }

                if (info.IsLiteral && !info.IsInitOnly)
                {
                    this.FindControl<Grid>("constIcon").IsVisible = true;
                }
                else
                {
                    this.FindControl<Grid>("fieldIcon").IsVisible = true;
                }

                this.FindControl<InputTextBox>("methodNameBox").Text = info.Name;

                if (info.FieldType != typeof(void))
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = info.FieldType.Name;
                    ToolTip.SetTip(this.FindControl<InputTextBox>("methodTypeBox"), info.FieldType.FullName);
                }
                else
                {
                    this.FindControl<InputTextBox>("methodTypeBox").Text = "void";
                    this.FindControl<InputTextBox>("methodTypeBox").Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                }
            }
        }
    }
}
