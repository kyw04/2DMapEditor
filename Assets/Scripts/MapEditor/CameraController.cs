using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace MapEditor
{
    public class CameraController : MonoBehaviour
    {
        public Camera cam;

        [Header("Pan")]
        public float panSpeed = 1.0f;
        public bool invertPan;
        public bool lockX;
        public bool lockY;

        [Header("Zoom")]
        public float pinchZoomSpeed;
        public float minOrthoSize = 2f;
        public float maxOrthoSize = 50f;
        public float minDistance = 2f;
        public float maxDistance = 50f;
        
        private Transform followTarget;
        private Vector3 targetPosition;
        private Vector2 lastSinglePos;
        private Vector2 lastTouch0, lastTouch1;
        private float targetOrthoSize;
        private float targetDistance;
        private bool isDragging;
        private bool hadTwoTouchesLastFrame;
        
        private void Reset()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            if (cam == null) cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("CameraController: camera is null. Disabling.");
                enabled = false;
                return;
            }
            
            targetPosition = followTarget != null ? followTarget.position : cam.transform.position;

            if (cam.orthographic)
            {
                targetOrthoSize = cam.orthographicSize;
            }
            else
            {
                targetDistance = (followTarget != null)
                    ? Vector3.Distance(cam.transform.position, followTarget.position)
                    : Vector3.Distance(cam.transform.position, Vector3.zero);
                Vector3 e = cam.transform.eulerAngles;
                
            }
        }

        private void Update()
        {
            TouchHandleManager(Touch.activeTouches.Count);
        }

        private void TouchHandleManager(int count)
        {
            switch (count)
            {
                case 1: PanHandle(); break;
                case 2: ZoomHandle(); break;
            }
        }

        private void PanHandle()
        {
            Touch t = Touch.activeTouches[0];
            Vector2 pos = t.screenPosition;

            if (t.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastSinglePos = pos;
                hadTwoTouchesLastFrame = false;
            }
            else if (t.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = pos - lastSinglePos;
                lastSinglePos = pos;

                float dir = invertPan ? 1f : -1f;
                Vector3 pan = ScreenDeltaToWorld(delta, dir);
                if (lockX) pan.x = 0f;
                if (lockY) pan.y = 0f;
                targetPosition += pan;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        private void ZoomHandle()
        {
            Touch t0 = Touch.activeTouches[0];
            Touch t1 = Touch.activeTouches[1];
            Vector2 cur0 = t0.screenPosition;
            Vector2 cur1 = t1.screenPosition;

            if (!hadTwoTouchesLastFrame)
            {
                lastTouch0 = cur0;
                lastTouch1 = cur1;
                hadTwoTouchesLastFrame = true;
                isDragging = false;
            }
            else
            {
                float prevDist = (lastTouch0 - lastTouch1).magnitude;
                float curDist = (cur0 - cur1).magnitude;
                float deltaDist = curDist - prevDist;

                if (cam.orthographic)
                {
                    targetOrthoSize -= deltaDist * pinchZoomSpeed;
                    targetOrthoSize = Mathf.Clamp(targetOrthoSize, minOrthoSize, maxOrthoSize);
                }
                else
                {
                    targetDistance -= deltaDist * pinchZoomSpeed;
                    targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
                }

                Vector2 prevMid = (lastTouch0 + lastTouch1) * 0.5f;
                Vector2 curMid = (cur0 + cur1) * 0.5f;
                Vector2 midDelta = curMid - prevMid;
                float dir = invertPan ? 1f : -1f;
                Vector3 pan = ScreenDeltaToWorld(midDelta, dir);
                if (lockX) pan.x = 0f;
                if (lockY) pan.y = 0f;
                targetPosition += pan;

                lastTouch0 = cur0;
                lastTouch1 = cur1;
            }
        }
        
        Vector3 ScreenDeltaToWorld(Vector2 deltaPixels, float dir)
        {
            float screenHeight = Mathf.Max(1f, Screen.height);
            Vector3 right = followTarget.right;
            Vector3 up = followTarget.up;

            float scale;
            if (cam.orthographic)
            {
                scale = (cam.orthographicSize * 2f) / screenHeight;
            }
            else
            {
                float dist = followTarget != null ? Vector3.Distance(cam.transform.position, followTarget.position) : 10f;
                scale = (dist * 2f) / screenHeight;
            }

            Vector3 pan = (right * deltaPixels.x + up * deltaPixels.y) * (scale * panSpeed * dir);
            return pan;
        }
    }
}