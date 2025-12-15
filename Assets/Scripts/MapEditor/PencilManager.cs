using UnityEngine;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public Drawable pencil;

        private void Start()
        {
            SelectPencil(pencil);
        }

        private void Update()
        {
            pencil.Draw();
        }

        public void SelectPencil(Drawable pen)
        {
            pencil = pen;
        }
    }
}
