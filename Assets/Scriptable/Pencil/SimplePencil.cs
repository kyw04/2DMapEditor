using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Pencil", order = 2)]
    public class SimplePencil : Drawable
    {
        public override void Begin() { }
        public override void End() { }

        protected override GameObject CreateRender(Vector3 pos)
        {
            var render = Instantiate(defaultObj, pos, Quaternion.identity).GetComponent<SpriteRenderer>();
            render.sprite = sprite;

            return render.gameObject;
        }

        protected override GameObject ChangeRender(Vector3 pos)
        {
            return null;
        }

        
    }
}
