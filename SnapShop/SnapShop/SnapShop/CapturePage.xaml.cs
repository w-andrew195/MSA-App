using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SnapShop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CapturePage : ContentPage
    {
        public CapturePage()
        {
            InitializeComponent();
            image.Source = ImageSource.FromFile("logo.png");
        }
        string placeName = "";
        private async void CameraButton(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Alert", "No camera available.", "Ok");
                return;
            }
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Small,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });
            if (file == null)
                return;
            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            placeName = "Please Wait...";
            TagLabel.Text = placeName;
            file.Dispose();
        }


    }
}