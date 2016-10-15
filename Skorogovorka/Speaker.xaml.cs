using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Media.Capture;
using Windows.Media.Transcoding;
using Newtonsoft.Json;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Skorogovorka
{
    public class ParsedJson
    {
        public double version { get; set; }
        public Header header { get; set; }
        public Result[] results { get; set; }

        public class Result
        {
            public string scenario { get; set; }
            public string name { get; set; }
            public string lexical { get; set; }
            public double confidence { get; set; }
        }

        public class Header
        {
            public class Propertie
            {
                public string requestId { get; set; }
                public string LOWCONF { get; set; }
            }


            public string status { get; set; }
            public string scenario { get; set; }
            public string name { get; set; }
            public string lexical { get; set; }

            public Propertie properties { get; set; }
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Speaker : Page
    {
        private bool recordButtonPushed = false;
        private string accessToken = "";

        private AudioGraph graph;
        private AudioFileOutputNode fileOutputNode;
        private AudioDeviceInputNode deviceInputNode;
        private string path;

        public Speaker()
        {
            this.InitializeComponent();
            string responce = SpeechApiInit();
            accessToken = responce;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string patter = (string)e.Parameter;
                textBlock.Text = patter;
            }
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

        private async void RecordStart(object sender, TappedRoutedEventArgs e)
        {
            if (!recordButtonPushed)
            {
                recordButtonPushed = true;
                await CreateAudioGraph();
                graph.Start();
            }
            else
            {
                recordButtonPushed = false;

                graph.Stop();

                TranscodeFailureReason finalizeResult = await fileOutputNode.FinalizeAsync();
                if (finalizeResult != TranscodeFailureReason.None)
                {
                    // Finalization of file failed. Check result code to see why
                    return;
                }

                Guid requestId = Guid.NewGuid();
                var Uri = @"https://speech.platform.bing.com/recognize?version=3.0&requestid=" + requestId.ToString() + @"&appID=D4D52672-91D7-4C74-8AD8-42B1D981415A&format=json&locale=en-US&device.os=Windows%20OS&scenarios=ulm&instanceid=f1efbd27-25fd-4212-9332-77cd63176112";

                var resp = SendRequestAsync(Uri, accessToken, "audio/wav; samplerate=16000", path);
                string json = resp;
                ParsedJson jsonResp = JsonConvert.DeserializeObject<ParsedJson>(json);
                json = jsonResp.header.lexical.Replace("<profanity>", "");
                json = json.Replace("</profanity>", "");
                var precise = StringDifference(textBlock.Text, json, jsonResp.results[0].confidence);
                Result.Text = json+"\nТочность "+precise.ToString("F1") + " %";
            }
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


        private async Task CreateAudioGraph()
        {
            if (graph != null)
            {
                graph.Dispose();
            }

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.SystemDefault;

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            graph = result.Graph;

            // Create a device input node using the default audio input device (manifest microphone!!!!)
            CreateAudioDeviceInputNodeResult deviceInputNodeResult = await graph.CreateDeviceInputNodeAsync(MediaCategory.Other);

            if (deviceInputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device input node
                return;
            }

            deviceInputNode = deviceInputNodeResult.DeviceInputNode;

            //creating file

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("sample.wav", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            path = file.Path.ToString();

            MediaEncodingProfile fileProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);

            // Operate node at the graph format, but save file at the specified format
            CreateAudioFileOutputNodeResult fileOutputNodeResult = await graph.CreateFileOutputNodeAsync(file, fileProfile);

            if (fileOutputNodeResult.Status != AudioFileNodeCreationStatus.Success)
            {
                // FileOutputNode creation failed
                return;
            }

            fileOutputNode = fileOutputNodeResult.FileOutputNode;

            // Connect the input node to both output nodes
            deviceInputNode.AddOutgoingConnection(fileOutputNode);
        }

        public double StringDifference(string example, string compare, double conf)
        {      
            example = example.Replace(',', ' ');
            example = example.Replace('.', ' ');
            example = example.Replace('?', ' ');
            example = example.Replace('!', ' ');
            example = example.Replace("\n", " ");
            example = example.Replace("  ", " ");
            example = example.Trim(' ');

            string[] ex = example.Split(' ');
            string[] cmp = compare.Split(' ');

            if (cmp.Length == 0)
            {
                return 0;
            }

            double diff = 100.0 / (example.Length - (ex.Length - 1));
            double result = 100;
            if (ex.Length >= cmp.Length)
            {
                int count = 0;
                foreach (string element in ex)
                {
                    if (count + 1 > cmp.Length)
                    {
                        count--;
                    }
                    if (string.Compare(element, cmp[count++], true) != 0)
                    {
                        result -= diff * element.Length;
                    }
                }
            }
            else
            {
                int count = 0;
                foreach (string element in ex)
                {
                    if (count + 1 > cmp.Length) { count = cmp.Length - 1; };
                    if (element.Length == cmp[count].Length)
                    {
                        if (string.Compare(element, cmp[count], true) != 0)
                        {
                            result -= diff * element.Length;
                            count++;
                        }
                    }
                    else
                    {
                        if (element.Length < cmp[count++].Length)
                        {
                            result -= diff * element.Length;
                        }
                        else
                        {
                            result -= diff * element.Length;
                            count++;
                        }
                    }
                }
            }

            return result * conf;
        }
    }
}
