using UnityEngine;
using UnityEngine.UI; // Required for UI elements like RawImage
using System.IO; // Required for File.ReadAllBytes

public class ImagePicker : MonoBehaviour
{
    // Assign a RawImage component from your UI to this field in the Inspector
    public Image image; 

    public void OnSelectImageButtonClicked()
    {
        // Prompt the user to select an image from the gallery
        // The callback function will be executed when an image is selected
        NativeGallery.GetImageFromGallery((imagePath) =>
        {
            if (imagePath != null)
            {
                // Create Texture2D from the selected image path
                // NativeGallery.LoadImageAtPath handles image loading and rotation based on EXIF data
                Texture2D texture = NativeGallery.LoadImageAtPath(imagePath, maxSize: 1024); 

                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + imagePath);
                    return;
                }
                
                // Assign the texture to the RawImage component in the UI
                if (image != null)
                {
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                Debug.Log(imagePath);
            }
        }, "Select an image", "image/*"); // Title of the gallery picker and MIME type filter
    }

    public void OnLoadImageButtonClicked(string path)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (image != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}