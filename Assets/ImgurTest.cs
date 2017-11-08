using UnityEngine;

// Execute this script in edit mode so the OnImageUploaded method is subscribed
// to the event without having to enter play mode every time
[ExecuteInEditMode]
public class ImgurTest : MonoBehaviour
{
    private ImgurClient imgurClient = new ImgurClient("");

    private void OnEnable()
    {
        imgurClient.OnImageUploaded += ImgurClient_OnImageUploaded;
    }

    private void ImgurClient_OnImageUploaded(object sender, ImgurClient.OnImageUploadedEventArgs e)
    {
        Debug.Log("Upload Complete: " + e.response.data.link);
    }

    [ContextMenu("Upload Image")]
    public void UploadImage()
    {
        imgurClient.UploadImageFromFilePath("PathToSomeImage.png");
    }

    [ContextMenu("Screenshot and Upload Image (Dont Delete Local File)")]
    public void ScreenshotAndDontDelete()
    {
        imgurClient.TakeAndUploadScreenshot("test.png", false);
    }

    [ContextMenu("Screenshot and Upload Image (Delete Local File)")]
    public void ScreenshotAndDelete()
    {
        imgurClient.TakeAndUploadScreenshot("test.png", true);
    }
}
