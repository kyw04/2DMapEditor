using System;
using UnityEngine;

public class GalleryToStorage : MonoBehaviour
{
    public void PickAndSave()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path)) return;
        
            Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize: 2048);
            if (tex == null)
            {
                // fallback: SaveFromPath (LoadImage bytes 내부에서 처리)
                string storedNameFallback = ImageStorageManager.Instance.SaveFromPath(path, "player_image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), useJpg: false, jpgQuality: 85, maxSize: 1024);
                if (!string.IsNullOrEmpty(storedNameFallback))
                {
                    CreateFromStored(storedNameFallback);
                }
                return;
            }

            string storedName = ImageStorageManager.Instance.SaveTexture(tex, "player_image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            // 즉시 게임에 생성
            Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        }, "Select an image");
    }

    public void CreateFromStored(string storedFileName)
    {
        Sprite sp = ImageStorageManager.Instance.LoadSprite(storedFileName, Vector2.one * 0.5f);
        if (sp == null) { Debug.LogWarning("Stored image not found: " + storedFileName); return; }
        
    }
}
