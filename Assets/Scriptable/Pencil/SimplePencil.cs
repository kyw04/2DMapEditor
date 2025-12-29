using UnityEngine;


namespace MapEditor.Pencil
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Pencil", fileName = "Pencil", order = 1)]
    public class SimplePencil : Drawable
    {
        protected override GameObject CreateRender(Vector3 pos)
        {
            var render = Instantiate(defaultObj, pos, Quaternion.identity).GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            render.color = color;

            return render.gameObject;
        }

        protected override GameObject ChangeRender(Vector3 pos)
        {
            return null;
        }
    }
}
