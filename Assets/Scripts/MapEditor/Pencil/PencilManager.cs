using UnityEngine;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public Drawable pencil;

        private void Update()
        {
            pencil.Draw();
        }
    }
}
