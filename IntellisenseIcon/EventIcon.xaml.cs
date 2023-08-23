using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IntellisenseIcon
{
    public partial class EventIcon : UserControl
    {
        public EventIcon()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
