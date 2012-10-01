using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Vasily.Models;
using Vasily.ThirdParty;
using Vasily.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Vasily
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Vasily.Common.LayoutAwarePage
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new MainPageViewModel();
                DataContext = ViewModel;
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            ViewModel.LoadFavorites();

            if (pageState != null && pageState.ContainsKey(Constants.MainPageState))
            {
                string serializedState = pageState[Constants.MainPageState].ToString();
                var state = SerializationHelper.DeserializeFromString<MainPageState>(serializedState);

                ViewModel.LoadState(state);
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.MainPageState] = serializedState;
        }

        private void HostNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string text = HostNameTextBox.Text;

            if (0 == String.Compare(text, MainPageViewModel.DefaultHostnamePlaceholder, StringComparison.OrdinalIgnoreCase))
            {
                HostNameTextBox.SelectAll();
            }
        }
    }
}
