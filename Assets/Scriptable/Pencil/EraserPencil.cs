using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Eraser Pencil", fileName = "Eraser Pencil", order = 4)]
    public class EraserPencil : Drawable
    {
        public override void Begin() { }

        public override void End() { }

        protected override RenderData CreateRender(Vector3 pos)
        {
            return null;
        }

        protected override RenderData ChangeRender(RenderData renderData, Vector3 pos)
        {
            if (renderData.isErased)
                return null;
            
            renderData.Disable();
            return renderData;
        }
    }
}
