using System;
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
        [Serializable]
        public struct PencilData
        {
            public string pencilName;
            public Drawable pencil;
        }

        public List<PencilData> pencilDatas;

        public void ButtonSetting(ScrollViewManager scrollViewManager)
        {
            Button[] children = scrollViewManager.objectViewContent.GetComponentsInChildren<Button>();
            for (int i = 0; i < children.Length; i++)
            {
                if (i < pencilDatas.Count)
                {
                    int index = i;
                    children[i].onClick.RemoveAllListeners();
                    children[i].onClick.AddListener(() =>
                    {
                        scrollViewManager.pencilManager.SelectPencil(pencilDatas[index].pencil);
                    });
                    children[i].GetComponentInChildren<TextMeshProUGUI>().text = pencilDatas[index].pencilName;
                }
                else
                {
                    children[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

