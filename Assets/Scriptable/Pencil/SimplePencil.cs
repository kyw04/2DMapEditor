using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Simple Pencil", order = 2)]
    public class SimplePencil : Drawable
    {
        public override void Begin() { }
        public override void End() { }

        protected override RenderData CreateRender(Vector3 pos)
        {
            return Instantiate(data.defaultObj, pos, Quaternion.identity);
        }

        protected override RenderData ChangeRender(RenderData hitData, Vector3 pos)
        {
            return null;
        }

        
    }
}
