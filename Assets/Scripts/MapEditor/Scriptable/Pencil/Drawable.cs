using UnityEngine;
using UnityEngine.EventSystems;

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
        
        public GameObject Draw(Vector3 pos)
        {
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y);
                
            if (Physics2D.Raycast(pos, Vector3.forward) ||
                EventSystem.current.IsPointerOverGameObject())
                return null;

            pos.z = 0;
            return Render(pos);
        }

        protected abstract GameObject Render(Vector3 pos);
    }
}
