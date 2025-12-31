using UnityEngine;
using UnityEngine.Android;

public class GalleryPermissionHandler : MonoBehaviour
{
    void Start()
    {
        CheckAndRequestPermissions();
    }

    void CheckAndRequestPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && Application.platform == RuntimePlatform.Android)
        {
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Debug.Log("갤러리 접근 권한 허용됨");
            }
            else
            {
                Debug.Log("갤러리 접근 권한 거부됨");
            }
        }
    }
}