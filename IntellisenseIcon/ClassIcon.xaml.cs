using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntellisenseIcon
{
    public partial class ClassIcon : UserControl
    {
        public ClassIcon()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
