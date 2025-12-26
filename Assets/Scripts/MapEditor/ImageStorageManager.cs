using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImageMeta
{
    public string fileName;
    public int width;
    public int height;
    public string savedAt;
}

public class ImageStorageManager : MonoBehaviour
{
    public static ImageStorageManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    string GetImagesFolder()
    {
        string folder = Path.Combine(Application.persistentDataPath, "SavedImages");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        return folder;
    }

    string GetImagePath(string fileName) => Path.Combine(GetImagesFolder(), fileName);
    string GetMetaPath(string fileName) => GetImagePath(fileName) + ".meta.json";

    // Texture2D를 저장 (자동으로 readable 여부 처리)
    public string SaveTexture(Texture2D tex, string baseName, bool useJpg = false, int jpgQuality = 85)
    {
        if (tex == null) throw new ArgumentNullException(nameof(tex));

        // 만약 tex.isReadable가 false라면 복사본을 만들어 사용
        Texture2D toSave = tex;
        if (!tex.isReadable)
        {
            toSave = TextureUtils.MakeTextureReadable(tex);
            if (toSave == null) throw new Exception("Failed to make texture readable");
        }

        string ext = useJpg ? ".jpg" : ".png";
        string safeName = MakeSafeFileName(baseName) + ext;
        string path = GetImagePath(safeName);
        Debug.Log(path);
        byte[] bytes = useJpg ? toSave.EncodeToJPG(jpgQuality) : toSave.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        ImageMeta meta = new ImageMeta
        {
            fileName = safeName,
            width = toSave.width,
            height = toSave.height,
            savedAt = DateTime.UtcNow.ToString("o")
        };
        string metaJson = JsonUtility.ToJson(meta);
        File.WriteAllText(GetMetaPath(safeName), metaJson);

        // 만약 복사본을 만든 경우 메모리 해제
        if (toSave != tex) UnityEngine.Object.Destroy(toSave);

        return safeName;
    }

    // 파일 경로로부터 바로 저장 (예: NativeGallery에서 얻은 path)
    public string SaveFromPath(string pathOnDevice, string baseName, bool useJpg = false, int jpgQuality = 85, int maxSize = 0)
    {
        if (string.IsNullOrEmpty(pathOnDevice) || !File.Exists(pathOnDevice)) return null;

        // 바이트로 읽어 LoadImage 사용하면 readable 텍스처 생성
        byte[] bytes = File.ReadAllBytes(pathOnDevice);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(bytes)) return null;

        // optional: maxSize로 리사이즈
        Texture2D finalTex = tex;
        if (maxSize > 0 && (tex.width > maxSize || tex.height > maxSize))
        {
            float scale = Mathf.Min((float)maxSize / tex.width, (float)maxSize / tex.height);
            int newW = Mathf.RoundToInt(tex.width * scale);
            int newH = Mathf.RoundToInt(tex.height * scale);
            Texture2D resized = new Texture2D(newW, newH, TextureFormat.RGBA32, false);
            // 간단한 리사이즈: Graphics.Blit via RenderTexture
            RenderTexture rt = RenderTexture.GetTemporary(newW, newH, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(tex, rt);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            resized.ReadPixels(new Rect(0, 0, newW, newH), 0, 0);
            resized.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            UnityEngine.Object.Destroy(tex);
            finalTex = resized;
        }

        string storedName = SaveTexture(finalTex, baseName, useJpg, jpgQuality);

        if (finalTex != tex) UnityEngine.Object.Destroy(finalTex);

        return storedName;
    }

    public Texture2D LoadTexture(string storedFileName)
    {
        string path = GetImagePath(storedFileName);
        if (!File.Exists(path)) return null;
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(bytes)) return null;
        return tex;
    }

    public Sprite LoadSprite(string storedFileName, Vector2 pivot)
    {
        Texture2D tex = LoadTexture(storedFileName);
        if (tex == null) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot);
    }

    public ImageMeta GetMeta(string storedFileName)
    {
        string metaPath = GetMetaPath(storedFileName);
        if (!File.Exists(metaPath)) return null;
        string json = File.ReadAllText(metaPath);
        return JsonUtility.FromJson<ImageMeta>(json);
    }

    public List<ImageMeta> ListSavedImages()
    {
        var list = new List<ImageMeta>();
        string folder = GetImagesFolder();
        var files = Directory.GetFiles(folder);
        foreach (var f in files)
        {
            if (f.EndsWith(".meta.json"))
            {
                string json = File.ReadAllText(f);
                var meta = JsonUtility.FromJson<ImageMeta>(json);
                if (meta != null) list.Add(meta);
            }
        }
        return list;
    }

    public bool DeleteSavedImage(string storedFileName)
    {
        try
        {
            string path = GetImagePath(storedFileName);
            string meta = GetMetaPath(storedFileName);
            if (File.Exists(path)) File.Delete(path);
            if (File.Exists(meta)) File.Delete(meta);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("DeleteSavedImage error: " + e);
            return false;
        }
    }

    string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
