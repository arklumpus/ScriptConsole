using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptConsoleLibrary
{
    internal class InputTextBox : TextBox, IStyleable
    {
        Type IStyleable.StyleKey => typeof(TextBox);

        public bool MyReadOnly
        {
            get { return (bool)GetValue(MyReadOnlyProperty); }
            set { SetValue(MyReadOnlyProperty, value); }
        }

        public static readonly AvaloniaProperty MyReadOnlyProperty = AvaloniaProperty.Register<InputTextBox, bool>("MyReadOnly");


        public EventHandler<KeyEventArgs> PreviewKeyDown
        {
            get { return (EventHandler<KeyEventArgs>)GetValue(PreviewKeyDownProperty); }
            set { SetValue(PreviewKeyDownProperty, value); }
        }

        public static readonly AvaloniaProperty PreviewKeyDownProperty = AvaloniaProperty.Register<InputTextBox, EventHandler<KeyEventArgs>>("PreviewKeyDown");


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.A && (e.Modifiers & InputModifiers.Control) == InputModifiers.Control)
            {
                SelectAll();
            }
            
            PreviewKeyDown?.Invoke(this, e);

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            if (!MyReadOnly)
            {
                base.OnTextInput(e);
            }
            else
            {
                e.Handled = true;
            }
        }

        private void SelectAll()
        {
            SelectionStart = 0;
            SelectionEnd = Text?.Length ?? 0;
        }

    }
}
