using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.SpeechRecognition;
using System.IO;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Skorogovorka
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool recordButtonPushed = false;
        private string accessToken = "";

        public MainPage()
        {
            this.InitializeComponent();
            string responce = SpeechApiInit();
            accessToken = responce;
        }

        private string SpeechApiInit()
        {
            // Create a client
            HttpClient httpClient = new HttpClient();

            // Add a new Request Message
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
            requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", "2dfda230f1224754a35ced999038f13d");
            HttpResponseMessage response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult(); ;
            string responseAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return responseAsString;
        }

        private void RecordStart(object sender, TappedRoutedEventArgs e)
        {
            Guid requestId = Guid.NewGuid();
            var Uri = @"https://speech.platform.bing.com/recognize?version=3.0&requestid=" + requestId.ToString() + @"&appID=D4D52672-91D7-4C74-8AD8-42B1D981415A&format=json&locale=en-US&device.os=Windows%20OS&scenarios=ulm&instanceid=f1efbd27-25fd-4212-9332-77cd63176112";

            var resp = SendRequestAsync(Uri, accessToken, "audio/wav; samplerate=16000", "Assets/whatstheweatherlike.wav");
            //textBlock.Text = resp;
        }

        public string SendRequestAsync(string url, string bearerToken, string contentType, string fileName)
        {
            var content = new StreamContent(File.OpenRead(fileName));
            content.Headers.TryAddWithoutValidation("Content-Type", contentType);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var response = httpClient.PostAsync(url, content).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
