using UnityEngine;

public static class TextureUtils
{
    // srcTexture가 readable하지 않아도 읽을 수 있는 Texture2D를 반환
    public static Texture2D MakeTextureReadable(Texture srcTexture)
    {
        if (srcTexture == null) return null;

        int width = srcTexture.width;
        int height = srcTexture.height;

        // RenderTexture 생성
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        // 블릿으로 src -> rt
        Graphics.Blit(srcTexture, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        // 읽을 수 있는 Texture2D 생성 (RGBA32로 안전하게 생성)
        Texture2D readableTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }
}