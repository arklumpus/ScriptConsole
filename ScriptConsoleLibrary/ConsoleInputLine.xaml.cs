using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace ScriptConsoleLibrary
{
    internal class ConsoleInputLine : UserControl
    {
        public ConsoleInputLine()
        {
            this.InitializeComponent();
            (this.Content as Control).DataContext = this;
            this.FindControl<InputTextBox>("commandBox").PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string Command { get { return this.FindControl<InputTextBox>("commandBox").Text; } set { this.FindControl<TextBox>("commandBox").Text = value; } }

        public EventHandler EnterPressed
        {
            get { return (EventHandler)GetValue(EnterPressedProperty); }
            set { SetValue(EnterPressedProperty, value); }
        }

        public static readonly AvaloniaProperty EnterPressedProperty = AvaloniaProperty.Register<ConsoleInputLine, EventHandler>("EnterPressed");

        public EventHandler<HistoryRequestEventArgs> HistoryRequested
        {
            get { return (EventHandler<HistoryRequestEventArgs>)GetValue(HistoryRequestedProperty); }
            set { SetValue(HistoryRequestedProperty, value); }
        }

        public static readonly AvaloniaProperty HistoryRequestedProperty = AvaloniaProperty.Register<ConsoleInputLine, EventHandler<HistoryRequestEventArgs>>("HistoryRequested");

        public int historyIndex = 0;

        public void ResetHistoryIndex()
        {
            historyIndex = 0;
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((e.Modifiers & InputModifiers.Shift) != InputModifiers.Shift && (e.Modifiers & InputModifiers.Control) != InputModifiers.Control)
                {
                    EnterPressed?.Invoke(this, new EventArgs());
                    e.Handled = true;
                }
                else
                {
                    this.FindControl<InputTextBox>("commandBox").Height += 18;
                }
            }
            else if (e.Key == Key.Up && (this.FindControl<InputTextBox>("commandBox").Text == null || !this.FindControl<InputTextBox>("commandBox").Text.Contains("\n") || this.FindControl<InputTextBox>("commandBox").CaretIndex < this.FindControl<InputTextBox>("commandBox").Text.IndexOf("\n")))
            {
                historyIndex--;
                HistoryRequested?.Invoke(this, new HistoryRequestEventArgs(historyIndex, -1));
                e.Handled = true;
            }
            else if (e.Key == Key.Down && (this.FindControl<InputTextBox>("commandBox").Text == null ||  this.FindControl<InputTextBox>("commandBox").CaretIndex >= this.FindControl<InputTextBox>("commandBox").Text.LastIndexOf("\n")))
            {
                historyIndex = Math.Min(0, historyIndex + 1);
                HistoryRequested?.Invoke(this, new HistoryRequestEventArgs(historyIndex, +1));
                e.Handled = true;
            }
            
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.V && (e.Modifiers & InputModifiers.Control) == InputModifiers.Control) ||
                (e.Key == Key.X && (e.Modifiers & InputModifiers.Control) == InputModifiers.Control) ||
                e.Key == Key.Return || e.Key == Key.Delete || e.Key == Key.Back)
            {
                this.FindControl<InputTextBox>("commandBox").Height = this.FindControl<InputTextBox>("commandBox").Text.CountOccurences("\n") * 18 + 18;
                int cInd = this.FindControl<InputTextBox>("commandBox").CaretIndex;
                this.FindControl<InputTextBox>("commandBox").CaretIndex = 0;
                this.FindControl<InputTextBox>("commandBox").CaretIndex = cInd;
            }
        }
    }

    internal class HistoryRequestEventArgs : EventArgs
    {
        public int RequestedIndex { get; }
        public int Direction { get; }

        public HistoryRequestEventArgs(int index, int direction)
        {
            RequestedIndex = index;
            Direction = direction;
        }
    }
}
