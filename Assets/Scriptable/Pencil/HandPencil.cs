using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hand Pencil", fileName = "Hand Pencil", order = 3)]
    public class HandPencil : Drawable
    {
        private Vector2 lastPos;
        public override RenderData Draw(Vector2 pos)
        {
            if (lastPos == Vector2.zero)
                lastPos = pos;
            
            return CreateRender(0, pos);
        }

        public override void Begin() { lastPos = Vector3.zero; }
        public override void End() { }

        protected override RenderData CreateRender(int sortingOrder, Vector2 pos)
        {
            float dir = CameraController.Instance.invert ? 1f : -1f;
            Vector2 newPos = pos - lastPos;

            CameraController.Instance.Move(newPos, dir);

            lastPos = pos;
            return null;
        }

        protected override RenderData ChangeRender(RenderData renderData, Vector2 pos)
        {
            return CreateRender(0, pos);
        }

        
    }
}
