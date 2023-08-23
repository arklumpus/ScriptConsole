using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace ScriptConsoleLibrary
{
    internal class MyExpandable : UserControl
    {
        public EventHandler<BoolChangedEventArgs> ExpandedChanged
        {
            get { return (EventHandler<BoolChangedEventArgs>)GetValue(ExpandedChangedProperty); }
            set { SetValue(ExpandedChangedProperty, value); }
        }

        public static readonly AvaloniaProperty ExpandedChangedProperty = AvaloniaProperty.Register<MyExpandable, EventHandler<BoolChangedEventArgs>>("ExpandedChanged");


        bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                bool changed = _isExpanded != value;
                _isExpanded = value;
                this.FindControl<Path>("expandDownArrow").IsVisible = !value;
                this.FindControl<Path>("expandUpArrow").IsVisible = value;
                if (changed)
                {
                    ExpandedChanged?.Invoke(this, new BoolChangedEventArgs(value));
                }
            }
        }

        public Control Header
        {
            get { return (Control)GetValue(HeaderProperty); }
            set
            {
                SetValue(HeaderProperty, value);
                this.FindControl<Grid>("headerContainer").Children.Clear();
                this.FindControl<Grid>("headerContainer").Children.Add(value);
            }
        }

        public static readonly AvaloniaProperty HeaderProperty = AvaloniaProperty.Register<MyExpandable, Control>("Header");

        public MyExpandable()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void expandButton_PointerEnter(object sender, PointerEventArgs e)
        {
            this.FindControl<Ellipse>("expandEllipse").Stroke = new SolidColorBrush(Color.FromRgb(143, 185, 255));
            this.FindControl<Path>("expandDownArrow").Stroke = new SolidColorBrush(Color.FromRgb(84, 107, 145));
            this.FindControl<Path>("expandUpArrow").Stroke = new SolidColorBrush(Color.FromRgb(84, 107, 145));
            this.FindControl<Ellipse>("expandEllipse").Fill = new SolidColorBrush(Color.FromRgb(243, 249, 255));
        }

        private void expandButton_PointerLeave(object sender, PointerEventArgs e)
        {
            this.FindControl<Ellipse>("expandEllipse").Stroke = Brushes.Black;
            this.FindControl<Ellipse>("expandEllipse").Fill = Brushes.White;
            this.FindControl<Path>("expandDownArrow").Stroke = Brushes.Black;
            this.FindControl<Path>("expandUpArrow").Stroke = Brushes.Black;
        }

        private void expandButton_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            this.FindControl<Ellipse>("expandEllipse").Stroke = new SolidColorBrush(Color.FromRgb(82, 136, 226));
            this.FindControl<Path>("expandDownArrow").Stroke = new SolidColorBrush(Color.FromRgb(59, 84, 125));
            this.FindControl<Path>("expandUpArrow").Stroke = new SolidColorBrush(Color.FromRgb(59, 84, 125));
            this.FindControl<Ellipse>("expandEllipse").Fill = new SolidColorBrush(Color.FromRgb(217, 236, 255));
        }

        private void expandButton_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            this.FindControl<Ellipse>("expandEllipse").Stroke = new SolidColorBrush(Color.FromRgb(143, 185, 255));
            this.FindControl<Path>("expandDownArrow").Stroke = new SolidColorBrush(Color.FromRgb(84, 107, 145));
            this.FindControl<Path>("expandUpArrow").Stroke = new SolidColorBrush(Color.FromRgb(84, 107, 145));
            this.FindControl<Ellipse>("expandEllipse").Fill = new SolidColorBrush(Color.FromRgb(243, 249, 255));
            this.IsExpanded = !this.IsExpanded;
        }
    }

    public class BoolChangedEventArgs : EventArgs
    {
        public bool NewValue { get; set; }
        public BoolChangedEventArgs(bool newValue)
        {
            NewValue = newValue;
        }
    }
}

