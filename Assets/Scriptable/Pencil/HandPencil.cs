using UnityEngine;

namespace MapEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hand Pencil", fileName = "Pencil", order = 1)]
    public class HandPencil : Pencil.Drawable
    {
        protected override GameObject CreateRender(Vector3 pos)
        {
            return null;
        }

        protected override GameObject ChangeRender(Vector3 pos)
        {
            return CreateRender(pos);
        }
    }
}
