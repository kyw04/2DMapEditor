using System.Collections.Generic;
using MapEditor.Pencil;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PencilList", fileName = "Pencil List", order = 1)]
public class PencilList : ScriptableObject
{
    public Drawable pencil;
    public List<Sprite> sprites;
    private List<DrawableData> datas;

    public List<DrawableData> CreateDrawableData()
    {
        if (datas == null)
            datas = new List<DrawableData>();
        else
            datas.Clear();
        
        foreach (var sprite in sprites)
        {
            DrawableData data = new DrawableData(pencil, sprite.name) { sprite = sprite };
            data.SaveData();
            datas.Add(data);
        }

        return datas;
    }
}
