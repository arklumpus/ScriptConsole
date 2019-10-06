using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;

namespace ScriptConsoleLibrary
{
    internal class ErrorLine : UserControl
    {
        public ErrorLine()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ErrorLine(Exception ex, bool isWarning = false)
        {
            InitializeComponent();

            this.FindControl<MyExpandable>("expander").ExpandedChanged += ExpandedChanged;

            InputTextBox headerBox;

            this.FindControl<MyExpandable>("expander").Header = headerBox = new InputTextBox() { FontSize = 12, Background = null, BorderThickness = new Thickness(0, 0, 0, 0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Thickness(-5, 0, 0, 0), MyReadOnly = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

            if (isWarning)
            {
                this.FindControl<Canvas>("errorIcon").IsVisible = false;
                this.FindControl<Canvas>("warningIcon").IsVisible = true;
                this.FindControl<Canvas>("background").Background = new SolidColorBrush(Color.FromRgb(255, 251, 230));
                this.FindControl<Canvas>("border1").Background = new SolidColorBrush(Color.FromRgb(255, 245, 194));
                this.FindControl<Canvas>("border2").Background = new SolidColorBrush(Color.FromRgb(255, 245, 194));
                headerBox.Foreground = new SolidColorBrush(Color.FromRgb(149, 127, 81));
            }

            headerBox.Text = ex.Message;

            if (ex is CompilationErrorException)
            {
                CompilationErrorException err = ((CompilationErrorException)ex);
                if (err.Diagnostics.Length == 1)
                {
                    headerBox.Text = err.GetType().Name + ": " + err.Diagnostics[0].GetMessage();

                    FileLinePositionSpan lineSpan = err.Diagnostics[0].Location.GetMappedLineSpan();

                    this.FindControl<InputTextBox>("locationBox").Text = "    " + err.Diagnostics[0].Id + " at " + lineSpan.Path + (!string.IsNullOrEmpty(lineSpan.Path) ? ":" : "") + (lineSpan.StartLinePosition.Line + 1).ToString() + ":" + (lineSpan.StartLinePosition.Character + 1).ToString();
                    this.FindControl<InputTextBox>("lineBox").Text = "    " + err.Diagnostics[0].Location.SourceTree.GetText().Lines[lineSpan.StartLinePosition.Line].ToString().Replace("\t", "    ");
                    this.FindControl<InputTextBox>("positionBox").Text = "    " + new string(' ', err.Diagnostics[0].Location.SourceTree.GetText().Lines[lineSpan.StartLinePosition.Line].ToString().CountCharsUpToPosition(lineSpan.StartLinePosition.Character)) + "^";
                }
                else
                {
                    this.FindControl<Grid>("detailsGrid").Children.Clear();
                    this.FindControl<Grid>("detailsGrid").RowDefinitions.Clear();

                    headerBox.Text = err.GetType().Name + "[" + err.Diagnostics.Length.ToString() + "]";

                    for (int i = 0; i < err.Diagnostics.Length; i++)
                    {
                        this.FindControl<Grid>("detailsGrid").RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        this.FindControl<Grid>("detailsGrid").RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        this.FindControl<Grid>("detailsGrid").RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        this.FindControl<Grid>("detailsGrid").RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        InputTextBox message = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.2, 0, 0), MyReadOnly = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
                        Grid.SetRow(message, i * 4);
                        Grid.SetColumn(message, 1);

                        message.Text = "• " + err.Diagnostics[i].GetMessage();

                        InputTextBox location = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.2, 0, 0), MyReadOnly = true };
                        Grid.SetRow(location, i * 4 + 1);
                        Grid.SetColumn(location, 1);

                        FileLinePositionSpan lineSpan = err.Diagnostics[i].Location.GetMappedLineSpan();
                        location.Text = "    " + err.Diagnostics[i].Id + " at " + lineSpan.Path + (!string.IsNullOrEmpty(lineSpan.Path) ? ":" : "") + (lineSpan.StartLinePosition.Line + 1).ToString() + ":" + (lineSpan.StartLinePosition.Character + 1).ToString();

                        InputTextBox line = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.2, 0, 0), Margin = new Thickness(8, 4, 0, 0), MyReadOnly = true, Foreground = new SolidColorBrush(Color.FromRgb(0x3F, 0x48, 0xCC)) };
                        Grid.SetRow(line, i * 4 + 2);
                        Grid.SetColumn(line, 1);
                        line.Text = "    " + err.Diagnostics[i].Location.SourceTree.GetText().Lines[lineSpan.StartLinePosition.Line].ToString().Replace("\t", "    ");

                        InputTextBox position = new InputTextBox() { Background = null, BorderThickness = new Thickness(0), FontFamily = ScriptConsoleControl.RobotoMono, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top, Padding = new Thickness(0, 1.2, 0, 0), Margin = new Thickness(8, -5, 0, 0), MyReadOnly = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
                        Grid.SetRow(position, i * 4 + 3);
                        Grid.SetColumn(position, 1);
                        position.Text = "    " + new string(' ', err.Diagnostics[i].Location.SourceTree.GetText().Lines[lineSpan.StartLinePosition.Line].ToString().CountCharsUpToPosition(lineSpan.StartLinePosition.Character)) + "^";

                        this.FindControl<Grid>("detailsGrid").Children.Add(message);
                        this.FindControl<Grid>("detailsGrid").Children.Add(location);
                        this.FindControl<Grid>("detailsGrid").Children.Add(line);
                        this.FindControl<Grid>("detailsGrid").Children.Add(position);
                    }
                }
            }
            else
            {
                headerBox.Text = ex.GetType().Name + ": " + ex.Message;

                this.FindControl<Grid>("detailsGrid").Children.Clear();
                this.FindControl<Grid>("detailsGrid").RowDefinitions.Clear();
                this.FindControl<Grid>("detailsGrid").ColumnDefinitions.Clear();
                this.FindControl<Grid>("detailsGrid").Margin = new Thickness(20, 0, 0, 0);
                this.FindControl<Grid>("detailsGrid").Children.Add(CSharpOutputLine.contentElement(ex));
            }
        }

        private void ExpandedChanged(object sender, BoolChangedEventArgs e)
        {
            this.FindControl<Grid>("detailsGrid").IsVisible = e.NewValue;
        }

        public static Control getContent(Exception ex)
        {
            ErrorLine inner = new ErrorLine(ex);
            inner.FindControl<Grid>("mainContainer").Children.Remove(inner.FindControl<Expander>("expander"));
            inner.FindControl<Grid>("mainContainer").Children.Remove(inner.FindControl<Grid>("detailsGrid"));

            Grid grd = new Grid();
            grd.Children.Add(inner.FindControl<Expander>("expander"));
            grd.Children.Add(inner.FindControl<Grid>("detailsGrid"));

            return grd;
        }
    }
}
