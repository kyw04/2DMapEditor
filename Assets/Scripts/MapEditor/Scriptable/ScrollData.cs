using System.Collections.Generic;
using MapEditor.Pencil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ScrollData", fileName = "ScrollData", order = 1)]
    public class ScrollData : ScriptableObject
    {
        public List<string> buttonName;
        public List<Drawable> pencil;

        public void ButtonSetting(Transform content)
        {
            Button[] children = content.GetComponentsInChildren<Button>();
            for (int i = 0; i < children.Length; i++)
            {
                if (i < buttonName.Count)
                {
                    children[i].GetComponentInChildren<TextMeshProUGUI>().text = buttonName[i];
                    // children[i].onClick
                }
                else
                {
                    children[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

