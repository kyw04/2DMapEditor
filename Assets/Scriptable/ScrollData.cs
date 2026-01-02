using System;
using System.Collections.Generic;
using MapEditor.Pencil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ScrollData", fileName = "ScrollData", order = 0)]
    public class ScrollData : ScriptableObject
    {
        [Serializable]
        public struct PencilData
        {
            public string pencilName;
            public Drawable pencil;
            
            public PencilData(string str, Drawable pen)
            {
                pencilName = str;
                pencil = pen;
            }
        }

        public Sprite titleImage;
        public bool usePencilList;
        public PencilList pencilList;
        public List<PencilData> pencilDatas;

        public void ButtonSetting(ScrollViewManager scrollViewManager)
        {
            if (pencilDatas.Count == 0 && usePencilList)
            {
                foreach (var sprite in pencilList.sprites)
                {
                    pencilList.pencil.sprite = sprite;
                    PencilData pencilData = new PencilData(sprite.name, pencilList.pencil);
                    pencilDatas.Add(pencilData);
                }
            }
            
            Button[] children = scrollViewManager.objectViewContent.GetComponentsInChildren<Button>();
            for (int i = 0; i < children.Length; i++)
            {
                if (i < pencilDatas.Count)
                {
                    int index = i;
                    var pencilData = pencilDatas[index];
                    children[i].onClick.RemoveAllListeners();
                    children[i].onClick.AddListener(() =>
                    {
                        scrollViewManager.pencilManager.SelectPencil(pencilData.pencil);
                        scrollViewManager.ObjectViewOnOff(this);
                    });
                    children[i].GetComponentInChildren<Image>().sprite = pencilData.pencil.sprite;
                    children[i].GetComponentInChildren<TextMeshProUGUI>().text = pencilData.pencilName;
                }
                else
                {
                    children[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

