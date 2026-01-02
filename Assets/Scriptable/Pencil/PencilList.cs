using System.Collections.Generic;
using MapEditor.Pencil;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PencilList", fileName = "PencilList", order = 1)]
public class PencilList : ScriptableObject
{
    public Drawable pencil;
    public List<Sprite> sprites;

    public List<DrawableData> CreateDrawableData()
    {
        List<DrawableData> datas = new List<DrawableData>();
        foreach (var sprite in sprites)
        {
            pencil.data.sprite = sprite;
            DrawableData data = new DrawableData(pencil, sprite.name);
            data.SaveData();
            datas.Add(data);
        }

        return datas;
    }
}
