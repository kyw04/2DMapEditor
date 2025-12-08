using UnityEngine;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public Drawable pencil;
        public int size;
        public Color32 color;

        private void Start()
        {
            pencil.Setting(size, color);
        }

        private void Update()
        {
            pencil.Draw();
        }
    }
}
