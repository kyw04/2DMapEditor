using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEditor.Pencil
{
    public abstract class Drawable : ScriptableObject
    {
        public GameObject defaultObj;
        public Sprite sprite;
        public Color32 color;

        public virtual GameObject Draw(Vector3 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return null;
            
            pos = Camera.main.ScreenToWorldPoint(pos);
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y, 0);
            
            if (Physics2D.Raycast(pos, Vector3.forward))
                return ChangeRender(pos);
            
            return CreateRender(pos);
        }
        
        public abstract void End();
        protected abstract GameObject CreateRender(Vector3 pos);
        protected abstract GameObject ChangeRender(Vector3 pos);
    }
}
