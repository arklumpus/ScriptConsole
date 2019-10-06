using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ScriptConsoleLibrary
{
    internal class ReducedExpander : UserControl
    {
        public Grid _grid;
        public MyExpandable _expander;
        public Grid ChildrenContainer { get { return _grid; } }
        public Control Header { get { return _expander.Header; }  set { _expander.Header = value; } }

        public ReducedExpander()
        {
            this.InitializeComponent();
            _grid = this.FindControl<Grid>("_grid");
            _expander = this.FindControl<MyExpandable>("_expander");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.FindControl<MyExpandable>("_expander").ExpandedChanged += _ExpandedChanged;
        }

        private void _ExpandedChanged(object sender, BoolChangedEventArgs e)
        {
            this.FindControl<Grid>("_grid").IsVisible = e.NewValue;
        }
    }
}
