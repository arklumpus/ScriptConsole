using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntellisenseIcon
{
    public partial class EnumIcon : UserControl
    {
        public EnumIcon()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
