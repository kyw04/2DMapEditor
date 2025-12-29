using System;
using UnityEngine;

public class IOSGalleryExample : MonoBehaviour
{
    public GameObject prefab; // SpriteRenderer 또는 UI Image 가진 프리팹
    public Transform parent;

    // 갤러리에서 이미지 선택 후 저장 및 생성
    public void PickImageAndSave()
    {
        // NativeGallery 내부에서 권한 요청을 처리합니다.
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("No image selected or permission denied/limited.");
                // iOS 14+에서 '선택 제한'이 걸릴 수 있음. 필요 시 사용자 안내.
                return;
            }

            // 권장: LoadImageAtPath로 리사이즈 옵션 사용 (메모리 절약)
            Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize: 2048);
            if (tex == null)
            {
                Debug.LogError("Failed to load texture from path: " + path);
                return;
            }

            // 예: persistentDataPath에 저장
            string fileName = "player_image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(savePath, png);
            Debug.Log("Saved image to: " + savePath);

            // 게임에 즉시 생성
            Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            var go = Instantiate(prefab, parent);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = sp;
            var uiImg = go.GetComponent<UnityEngine.UI.Image>();
            if (uiImg != null) uiImg.sprite = sp;

            // 주의: tex를 Destroy하면 스프라이트가 깨질 수 있음. 필요 시 스프라이트 복사 전략 사용.
        }, "Select an image from your photo library");
    }

    // 이미지 저장(갤러리에 저장) 예시
    public void SaveTextureToGallery(Texture2D tex, string albumName = "MyApp")
    {
        if (tex == null) return;
        NativeGallery.SaveImageToGallery(tex, albumName, "saved_image.png", (success, path) =>
        {
            Debug.Log("Saved to gallery: " + success + " path: " + path);
        });
    }

    // 앱 설정 열기 (권한 거부 시 유도)
    public void OpenAppSettings()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        Application.OpenURL("app-settings:");
        #else
        Debug.Log("OpenAppSettings is only available on iOS device.");
        #endif
    }
}
