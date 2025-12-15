using Unity.VisualScripting;
using UnityEngine;

namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Pencil", order = 1)]
    public class SimplePencil : Drawable
    {
        protected override void Render(Vector3 pos)
        {
            var render = Instantiate(defaultObj, pos, Quaternion.identity).GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            render.color = color;
        }
    }
}
