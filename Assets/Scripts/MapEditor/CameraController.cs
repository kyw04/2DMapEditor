using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace MapEditor
{
    public class CameraController : MonoBehaviour
    {
        
        public Camera cam;

        [Header("Movement")]
        public float moveSpeed = 1f;
        public float moveSensitivity = 1f;
        public bool invert;
        public bool lockX;
        public bool lockY;

        [Header("Zoom")]
        public float pinchZoomSpeed;
        public float zoomSensitivity = 30f;
        public float minOrthoSize = 2f;
        public float maxOrthoSize = 50f;
        
        private Transform followTarget;
        private Vector3 targetPosition;
        private Vector2 lastTouch0, lastTouch1;
        private float targetOrthoSize;
        private float velocity;
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

            followTarget = cam.transform;
            targetPosition = followTarget != null ? followTarget.position : cam.transform.position;

            if (cam.orthographic)
            {
                targetOrthoSize = cam.orthographicSize;
            }
            else
            {
                Debug.LogWarning("CameraController: Camera projection is not \"Orthographic\".");
            }
        }

        private void Update()
        {
            TouchHandleManager(Touch.activeTouches.Count);
            cam.transform.position = targetPosition;
            cam.orthographicSize = targetOrthoSize;
        }

        private void TouchHandleManager(int count)
        {
            switch (count)
            {
                case 0: case 1: hadTwoTouchesLastFrame = false; break;
                case 2: MoveAndZoom(); break;
            }
        }


        private void MoveAndZoom()
        {
            Touch t0 = Touch.activeTouches[0];
            Touch t1 = Touch.activeTouches[1];
            Vector2 cur0 = t0.screenPosition;
            Vector2 cur1 = t1.screenPosition;
            Vector2 curMid = (cur0 + cur1) * 0.5f;

            if (!hadTwoTouchesLastFrame)
            {
                lastTouch0 = cur0;
                lastTouch1 = cur1;
                hadTwoTouchesLastFrame = true;
            }
            else
            {
                float prevDist = (lastTouch0 - lastTouch1).magnitude;
                float curDist = (cur0 - cur1).magnitude;
                float deltaDist = curDist - prevDist;
                Vector2 prevMid = (lastTouch0 + lastTouch1) * 0.5f;
                Vector2 midDelta = curMid - prevMid;
                
                if (midDelta.magnitude >= moveSensitivity)
                {
                    float dir = invert ? 1f : -1f;
                    Vector3 pan = ScreenDeltaToWorld(midDelta, dir);
                
                    if (lockX) pan.x = 0f;
                    if (lockY) pan.y = 0f;
                    targetPosition += pan;
                }

                if (Mathf.Abs(deltaDist) >= zoomSensitivity)
                {
                    targetOrthoSize -= deltaDist * pinchZoomSpeed;
                    targetOrthoSize = Mathf.Clamp(targetOrthoSize, minOrthoSize, maxOrthoSize);
                }
                
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

            Vector3 pan = (right * deltaPixels.x + up * deltaPixels.y) * (scale * moveSpeed * dir);
            return pan;
        }
    }
}