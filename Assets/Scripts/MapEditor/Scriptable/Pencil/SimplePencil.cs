using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Pencil", order = 1)]
    public class SimplePencil : Drawable
    {
        protected override void Render(Vector3 pos)
        {
            var render = Instantiate(obj, pos, Quaternion.identity).GetComponent<SpriteRenderer>();
            render.color = color;
        }
    }
}
