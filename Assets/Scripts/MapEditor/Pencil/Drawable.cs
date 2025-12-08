using Unity.Mathematics;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MapEditor.Pencil
{
    public abstract class Drawable : MonoBehaviour
    {
        public GameObject obj;
        public int size { get; private set; }
        public Color32 color { get; private set; }

        public void SetSize(int value)
        {
            size = value;
        }

        public void SetColor(Color32 value)
        {
            color = value;
        }

        public void Draw()
        {
            if (Touch.activeTouches.Count > 0)
            {
                Vector3 temp = Camera.main.ScreenToWorldPoint(Touch.activeTouches[0].screenPosition);
                float x = Mathf.Floor(temp.x);
                float y = Mathf.Floor(temp.y);
                Vector3 pos = new Vector3(x, y);
                
                if (Physics2D.Raycast(pos, Vector3.forward))
                    return;

                pos.z = 0;
                Render(pos);
            }
        }
        
        protected virtual void Render(Vector3 pos)
        {
            Instantiate(obj, pos, quaternion.identity);
        }
    }
}
