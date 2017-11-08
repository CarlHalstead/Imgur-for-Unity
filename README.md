# Imgur-for-Unity
Enables the use of the Imgur API for uploading images as well as methods for taking and uploading screenshots to Imgur.

## Getting Started
Firstly, you should create a Client ID from the Imgur site:
https://api.imgur.com/oauth2/addclient
This enables the script to authorise uploads to the API.

Secondly, you should create a new instance of the ImgurClient with the one and only parameter being your Client ID.

Now you can upload images!

(For more information see the ImgurTest.cs file here: https://github.com/GamingAnonymous/Imgur-for-Unity/blob/master/Assets/ImgurTest.cs)

### Methods
	* TakeAndUploadScreenshot - Takes a screenshot and uploads it to Imgur.
	* UploadImageFromFilePath - Takes the path to a file and turns into Base64 data to be uploaded.
	* UploadImage - Takes already converted Base64 data and is immediately uploaded.
