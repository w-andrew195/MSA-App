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
using SnapShop.Models;
using Plugin.Connectivity;
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
        private bool CheckNetwork() {

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
        string placeName = "";
        private async void CameraButton(object sender, EventArgs e)
        {
            bool Status = CheckNetwork();
            if (!Status) { return; }
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
                Name = $"SnapShop{DateTime.UtcNow}.jpg"
            });
            if (file == null)
                return;
            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            placeName = "Please Wait...";
            TagLabel.Text = placeName;
            await MakePredictionRequest(file);
            await postLocationAsync();
        }
        async Task postLocationAsync()
        {
            DateTime thisDay = DateTime.Now;

            SnapShopNZInformation model = new SnapShopNZInformation()
            {
                Date = thisDay.ToString("dd/MM/yy - HH:mm"),
                Place = placeName

            };

            await Azure.AzureManagerInstance.PostInformation(model);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "60b7c5c219ec4d8395e592511ffab18b");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/1bf4f18a-cbf8-4af8-a5ab-53a5dfd8ad79/image?iterationId=4ee881d3-3750-4966-98a9-c143d1f3c1eb";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    CVModel responseModel = JsonConvert.DeserializeObject<CVModel>(responseString);


                    placeName = responseModel.Predictions.ToList()[0].Tag;
                    TagLabel.Text = responseModel.Predictions.ToList()[0].Tag;
                    DataEntry DataOut = new DataEntry();
                    string website = DataOut.checker(placeName);
                    DataOutLabel.Text = website;



                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) => {

                        Device.OpenUri(new System.Uri(website));
                    };
                    DataOutLabel.GestureRecognizers.Add(tapGestureRecognizer);

                }

                file.Dispose();
            }
        }

    }
}