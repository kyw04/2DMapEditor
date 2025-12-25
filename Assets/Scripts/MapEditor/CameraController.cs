using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.EventSystems;

namespace MapEditor
{
    public class CameraController : MonoBehaviour
    {
        public Camera cam;
        public bool isMove;
        public bool isZoom;
        
        [Space]
        public bool lockX;
        public bool lockY;
        
        
        [Header("Movement")]
        public float moveSpeed = 1f;
        public bool invert;

        [Header("Zoom")]
        public float zoomSpeed;
        public float zoomSensitivity = 30f;
        public float minOrthoSize = 2f;
        public float maxOrthoSize = 50f;
        
        private Transform followTarget;
        private Vector2 lastTouchCenter;
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
            
        }

        private void TouchHandleManager(int count)
        {
            switch (count)
            {
                case 0: hadTwoTouchesLastFrame = false; break;
                case 2:
                    MoveAndZoom();
                    break;
            }
        }

        private void MoveAndZoom()
        {
            Touch t0 = Touch.activeTouches[0];
            Touch t1 = Touch.activeTouches[1];
            Vector2 cur0 = t0.screenPosition;
            Vector2 cur1 = t1.screenPosition;
            Vector2 curMid = (cur0 + cur1) * 0.5f;

            // Ray ray = cam.ScreenPointToRay(new Vector3(curMid.x, 0, curMid.y));
            // if (EventSystem.current.IsPointerOverGameObject())
            //     return;
                    
            if (!hadTwoTouchesLastFrame)
            {
                lastTouch0 = cur0;
                lastTouch1 = cur1;
                lastTouchCenter = (cur0 + cur1) * 0.5f;
                hadTwoTouchesLastFrame = true;
            }
            else
            {
                float dir = invert ? 1f : -1f;

                if (isMove)
                {
                    Vector2 prevMid = (lastTouch0 + lastTouch1) * 0.5f;
                    Vector2 delta = curMid - prevMid;
                    Vector3 deltaWorld = ScreenDeltaToWorld(delta, dir);
                    if (lockX) deltaWorld.x = 0f;
                    if (lockY) deltaWorld.y = 0f;
                    
                    followTarget.position += deltaWorld;
                }

                if (isZoom)
                {
                    float prevDis = Vector2.Distance(lastTouch0, lastTouch1);
                    float curDis = Vector2.Distance(cur0, cur1);
                    float delta = curDis - prevDis;

                    if (Mathf.Abs(delta) >= zoomSensitivity)
                    {
                        Vector2 moveDelta =  curMid - lastTouchCenter;
                        Vector3 deltaWorld = ScreenDeltaToWorld(moveDelta, dir);
                        if (lockX) deltaWorld.x = 0f;
                        if (lockY) deltaWorld.y = 0f;

                        followTarget.position += deltaWorld;
                        targetOrthoSize += delta * zoomSpeed * Time.deltaTime;
                        cam.orthographicSize = Mathf.Clamp(targetOrthoSize, minOrthoSize, maxOrthoSize);
                    }
                }
            }
            
            lastTouch0 = cur0;
            lastTouch1 = cur1;
            lastTouchCenter = curMid;
        }
        
        Vector3 ScreenDeltaToWorld(Vector2 deltaPixels, float dir)
        {
            float screenHeight = Mathf.Max(1f, Screen.height);
            Vector3 right = followTarget.right;
            Vector3 up = followTarget.up;

            float scale = (cam.orthographicSize * 2f) / screenHeight;
            Vector3 pan = (right * deltaPixels.x + up * deltaPixels.y) * (scale * moveSpeed * dir);
            return pan;
        }
    }
}