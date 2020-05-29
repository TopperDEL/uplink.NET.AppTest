using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uplink.NET.AppTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var version = uplink.NET.Models.Access.GetStorjVersion();
            storjVersion.Text = version;
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            using (var access = new uplink.NET.Models.Access("YOUR_SATELLITE_URL", "YOUR_API_KEY", "YOUR_SECRET"))
            {
                //Create the bucket
                var bucketService = new uplink.NET.Services.BucketService(access); //1
                var bucket = await bucketService.EnsureBucketAsync("test-app-bucket"); //2

                //Upload some bytes
                var bytesToUpload = Encoding.UTF8.GetBytes("Storj is awesome, isn't it?");
                var objectService = new uplink.NET.Services.ObjectService(access); //3
                var uploadOperation = await objectService.UploadObjectAsync(bucket, "test.txt", new UploadOptions(), bytesToUpload, false); //4
                await uploadOperation.StartUploadAsync(); //5

                //List the bucket-content
                var bucketContent = await objectService.ListObjectsAsync(bucket, new ListObjectsOptions() { Recursive = true }); //6
                foreach (var item in bucketContent.Items)
                {
                    await ShowMessageAsync("Found item: " + item.Key);
                }

                //Download some bytes
                var downloadOperation = await objectService.DownloadObjectAsync(bucket, "test.txt", new DownloadOptions(), false); //6
                await downloadOperation.StartDownloadAsync(); //7

                //Verify the bytes
                var receivedString = Encoding.UTF8.GetString(downloadOperation.DownloadedBytes); //8
                await ShowMessageAsync("Received string: " + receivedString);

                //Delete the object
                await objectService.DeleteObjectAsync(bucket, "test.txt"); //9

                //Delete the bucket
                await bucketService.DeleteBucketAsync(bucket.Name); //10
            }
        }

        private async Task ShowMessageAsync(string messageToShow)
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog(messageToShow, "Go, Storj, go!");
            await messageDialog.ShowAsync();
        }
    }
}
