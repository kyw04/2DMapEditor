using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MapEditor.Pencil
{
    public abstract class Drawable : ScriptableObject
    {
        public GameObject defaultObj;
        public Sprite sprite;
        public Color32 color;

        public void Setting(Color32 color)
        {
            this.color = color;
        }
        
        public GameObject Draw(Vector3 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return null;
            
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y, 0);
            
            if (Physics2D.Raycast(pos, Vector3.forward))
                return ChangeRender(pos);
            
            return CreateRender(pos);
        }

        protected abstract GameObject CreateRender(Vector3 pos);
        protected abstract GameObject ChangeRender(Vector3 pos);
    }
}
