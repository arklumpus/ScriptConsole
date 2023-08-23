using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntellisenseIcon
{
    public partial class UnknownIcon : UserControl
    {
        public UnknownIcon()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
