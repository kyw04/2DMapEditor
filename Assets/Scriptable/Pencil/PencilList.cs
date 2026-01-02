using System.Collections.Generic;
using MapEditor.Pencil;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PencilList", fileName = "PencilList", order = 1)]
public class PencilList : ScriptableObject
{
    public Drawable pencil;
    public List<Sprite> sprites;
}
