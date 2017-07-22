using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SnapShop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
            RetrieveInformation();
        }
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            bool Status = CheckNetwork();
            if (!Status) { return; }

            List<SnapShopNZInformation> SnapShopInformation = await Azure.AzureManagerInstance.GetInformation();

            SnapShopList.ItemsSource = SnapShopInformation;
        }
        async Task RetrieveInformation()
        {
            bool Status = CheckNetwork();
            if (!Status) { return; }

            List<SnapShopNZInformation> Information = await Azure.AzureManagerInstance.GetInformation();

            SnapShopList.ItemsSource = Information;
        }

        private bool CheckNetwork()
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {
                DisplayAlert("Warning", "Network connection required.", "Ok");
                return false;
            }
        }

    }
}