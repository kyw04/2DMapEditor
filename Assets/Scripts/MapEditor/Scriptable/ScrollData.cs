using System.Collections.Generic;
using MapEditor.Pencil;
using UnityEngine;

namespace MapEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ScrollData", fileName = "ScrollData", order = 1)]
    public class ScrollData : ScriptableObject
    {
        public string headerID;
        public List<string> buttonName;
        public List<Drawable> pencil;
    }
}

