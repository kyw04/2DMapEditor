using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MapEditor.Pencil
{
    public abstract class Drawable : ScriptableObject
    {
        public GameObject defaultObj;
        public Sprite sprite;
        public int size;
        public Color32 color;

        public void Setting(int size, Color32 color)
        {
            this.size = size;
            this.color = color;
        }
        
        public void Draw()
        {
            if (Touch.activeTouches.Count > 0)
            {
                Vector3 temp = Camera.main.ScreenToWorldPoint(Touch.activeTouches[0].screenPosition);
                float x = Mathf.Round(temp.x);
                float y = Mathf.Round(temp.y);
                Vector3 pos = new Vector3(x, y);
                
                if (Physics2D.Raycast(pos, Vector3.forward) ||
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                pos.z = 0;
                Render(pos);
            }
        }

        protected abstract void Render(Vector3 pos);
    }
}
