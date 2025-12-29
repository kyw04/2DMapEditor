#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class IOSPlistPostprocessor
{
    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;
        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        var root = plist.root;

        root.SetString("NSPhotoLibraryUsageDescription", "앱에서 사진을 선택하려면 갤러리 접근이 필요합니다.");
        root.SetString("NSPhotoLibraryAddUsageDescription", "앱에서 사진을 저장하려면 권한이 필요합니다.");
        root.SetString("NSCameraUsageDescription", "사진을 찍어 업로드하려면 카메라 접근이 필요합니다.");

        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif