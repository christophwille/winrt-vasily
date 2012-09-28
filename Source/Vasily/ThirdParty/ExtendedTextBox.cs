using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/775f1692-2837-471c-95fc-710bf0e9cc53
//
namespace Vasily.ThirdParty
{
    public class ExtendedTextBox : TextBox
    {
        public static readonly DependencyProperty CustomActionProperty =
            DependencyProperty.Register(
            "CustomAction",
            typeof(Action<string>),
            typeof(ExtendedTextBox),
            new PropertyMetadata(null, OnPropertyChanged));

        public Action<string> CustomAction
        {
            get
            {
                return (Action<string>)GetValue(CustomActionProperty);
            }
            set
            {
                SetValue(CustomActionProperty, value);
            }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (d as ExtendedTextBox).TextChanged += ExtendedTextBox_TextChanged;
            else
                (d as ExtendedTextBox).TextChanged -= ExtendedTextBox_TextChanged;
        }

        async static void ExtendedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => (sender as ExtendedTextBox).CustomAction((sender as ExtendedTextBox).Text));
        }
    }
}
