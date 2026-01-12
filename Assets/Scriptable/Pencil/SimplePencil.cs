using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Simple Pencil", order = 2)]
    public class SimplePencil : Drawable
    {
        public override void Begin() { }
        public override void End() { }

        protected override RenderData CreateRender(int sortingOrder, Vector2 pos)
        {
            MapData mapData = new MapData(pos, data.sprite, data.defaultObj, sortingOrder);
            return GameManager.Instance.CreateMap(mapData);
        }

        protected override RenderData ChangeRender(RenderData hitData, Vector2 pos)
        {
            MapData mapData = new MapData(pos, data.sprite, data.defaultObj);
            return GameManager.Instance.ChangeMap(hitData, mapData);
        }
    }
}
