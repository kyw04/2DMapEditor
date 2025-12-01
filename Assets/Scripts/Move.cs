using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditor
{
    public class Move : MonoBehaviour
    {
        private Transform cam;

        public bool isReverse;
        public float moveSpeed;
        
        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                var del = Mouse.current.delta.value;
                del = isReverse ? del : -del;
                cam.position += new Vector3(del.x, del.y) * (Time.deltaTime * moveSpeed);
            }
        }
    }
}