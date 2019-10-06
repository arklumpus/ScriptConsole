using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace ScriptConsoleLibrary
{
    internal class NamespaceStructure : UserControl
    {
        public NamespaceStructure()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public NamespaceStructure(string @namespace, string filter, bool showEverything, Type context, ScriptConsoleControl consoleContext)
        {
            InitializeComponent();
            NamespaceStructureExpander expander = new NamespaceStructureExpander(@namespace, filter, showEverything, context, consoleContext);
            Grid.SetRow(expander, 1);
            Grid.SetColumn(expander, 1);
            this.FindControl<Grid>("outputGrid").Children.Add(expander);
        }
    }
}
