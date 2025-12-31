using UnityEngine;

namespace MapEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hand Pencil", fileName = "Pencil", order = 1)]
    public class HandPencil : Pencil.Drawable
    {
        private Vector3 lastPos;

        public override GameObject Draw(Vector3 pos)
        {
            return CreateRender(pos);
        }
        
        public override void End()
        {
            lastPos = Vector3.zero;
        }

        protected override GameObject CreateRender(Vector3 pos)
        {
            if (lastPos == Vector3.zero)
                lastPos = pos;
                
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
