using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntellisenseIcon
{
    public partial class KeywordIcon : UserControl
    {
        public KeywordIcon()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
