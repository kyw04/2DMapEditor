using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Eraser Pencil", fileName = "Eraser Pencil", order = 4)]
    public class EraserPencil : Drawable
    {
        public override void Begin() { }

        public override void End() { }

        protected override RenderData CreateRender(Vector2 pos)
        {
            return null;
        }

        protected override RenderData ChangeRender(RenderData renderData, Vector2 pos)
        {
            if (renderData == null)
                return null;

            if (renderData.isErased)
                return null;

            GameManager.Instance.DeleteMap(renderData);
            return renderData;
        }
    }
}
