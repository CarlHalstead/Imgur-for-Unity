using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class ImgurClient
{
    /// <summary>
    /// This client ID is used when uploading images to Imgur and is mandatory to use the API
    /// </summary>
    private readonly string _clientId;

    /// <summary>
    /// URL to send the images for upload. Accepts a maximum file size of 10MB.
    /// </summary>
    private const string _baseUploadUrl = "https://api.imgur.com/3/upload";

    /// <summary>
    /// URL to create an application and retrieve your client ID
    /// </summary>
    private const string _baseApplicationRegisterUrl = "https://api.imgur.com/oauth2/addclient";

    /// <summary>
    /// When an image upload has completed this event is fired with the JSON returned as an object
    /// containing things like the link to the image you just uploaded
    /// </summary>
    public event EventHandler<OnImageUploadedEventArgs> OnImageUploaded;

    /// <summary>
    /// Create an instance of the ImgurClient and use the methods to take screenshots/
    /// upload images to the API.
    /// </summary>
    /// <param name="_clientId">Client ID to authorise requests to the API</param>
    public ImgurClient(string _clientId)
    {
        if (_clientId.Equals(string.Empty))
            Debug.LogError("You need a client ID to use the API! You can get one from here: " + _baseApplicationRegisterUrl);

        this._clientId = _clientId;

        ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
    }

    /// <summary>
    /// Take a screenshot of the current game view. Wait until it has saved to the disk on
    /// another thread so the Unity thread may continue. When it is finally saved, get the bytes from
    /// the file, convert it to Base64 and send it to the Imgur API.
    /// 
    /// If necessary, delete the file after upload so it is not kept on the users disk.
    /// </summary>
    /// <param name="screenshotName">Name of the screenshot (Including extension)</param>
    /// <param name="deleteLocalFileAfterUpload">Should the file be deleted from the disk after it has been uploaded</param>
    public void TakeAndUploadScreenshot(string screenshotName, bool deleteLocalFileAfterUpload)
    {
        if (Directory.Exists(Application.streamingAssetsPath) == false)
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        string filePath = Path.Combine(Application.streamingAssetsPath, screenshotName);

        ScreenCapture.CaptureScreenshot(filePath);

        Thread t = new Thread(() =>
        {
            while (File.Exists(filePath) == false)
            {
                Thread.Sleep(10);
            }

            byte[] image = File.ReadAllBytes(filePath);
            string base64Image = Convert.ToBase64String(image);

            Upload(base64Image, (response) =>
            {
                if (OnImageUploaded != null)
                {
                    OnImageUploaded(this, new OnImageUploadedEventArgs(response));
                }

                if (deleteLocalFileAfterUpload == true)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            });
        })
        {
            IsBackground = true
        };

        t.Start();
    }

    /// <summary>
    /// Take a file path and load it in as a byte array. Then convert
    /// it to a Base64 string so it can be uploaded to the Imgur API.
    /// </summary>
    /// <param name="filePath">)Path to the file to be uploaded</param>
    public void UploadImageFromFilePath(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            Debug.LogError("[Imgur] That file does not exist: " + filePath);
            return;
        }

        byte[] image = File.ReadAllBytes(filePath);
        UploadImage(Convert.ToBase64String(image));
    }

    /// <summary>
    /// Take the Base64 data and upload it to the Imgur API
    /// </summary>
    /// <param name="base64Image">Base64 data of an image/gif</param>
    public void UploadImage(string base64Image)
    {
        Upload(base64Image, (response) =>
        {
            if (OnImageUploaded != null)
            {
                OnImageUploaded(this, new OnImageUploadedEventArgs(response));
            }
        });
    }

    /// <summary>
    /// Upload Base64 data to the API, ensuring to call the action when it has completed so
    /// we can do things such as firing the OnImageUploaded event.
    /// </summary>
    /// <param name="base64Image"></param>
    /// <param name="OnUploadCompleted"></param>
    private void Upload(string base64Image, Action<ImgurUploadResponse> OnUploadCompleted)
    {
        Thread t = new Thread(() =>
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", "Client-ID " + _clientId);

                NameValueCollection parameters = new NameValueCollection()
                {
                    { "image", base64Image }
                };

                byte[] response = client.UploadValues(_baseUploadUrl, parameters);
                string json = Encoding.UTF8.GetString(response);

                OnUploadCompleted(JsonUtility.FromJson<ImgurUploadResponse>(json));
            }
        })
        {
            IsBackground = true
        };

        t.Start();
    }

    public class OnImageUploadedEventArgs : EventArgs
    {
        /// <summary>
        /// Reponse from the Imgur API containing attributes such as the link to the image
        /// </summary>
        public ImgurUploadResponse response;

        public OnImageUploadedEventArgs(ImgurUploadResponse response)
        {
            this.response = response;
        }
    }
}
