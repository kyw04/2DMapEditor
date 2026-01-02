using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hand Pencil", fileName = "Hand Pencil", order = 3)]
    public class HandPencil : Drawable
    {
        private Vector3 lastPos;
        public override GameObject Draw(Vector3 pos)
        {
            if (lastPos == Vector3.zero)
                lastPos = pos;
            
            return CreateRender(pos);
        }

        public override void Begin() { lastPos = Vector3.zero; }
        public override void End() { }

        protected override GameObject CreateRender(Vector3 pos)
        {
            float dir = CameraController.Instance.invert ? 1f : -1f;
            Vector2 newPos = pos - lastPos;

            CameraController.Instance.Move(newPos, dir);

            lastPos = pos;
            return null;
        }

        protected override GameObject ChangeRender(Vector3 pos)
        {
            return CreateRender(pos);
        }

        
    }
}
