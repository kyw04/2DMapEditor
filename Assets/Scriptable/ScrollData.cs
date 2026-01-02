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
            public DrawableData data;
            
            public PencilData(string str, DrawableData pen)
            {
                pencilName = str;
                data = pen;
            }
        }

        public Sprite titleImage;
        public bool usePencilList;
        public PencilList pencilList;
        public List<PencilData> pencilDatas;

        public void ButtonSetting(ScrollViewManager scrollViewManager)
        {
            if (usePencilList)
            {
                pencilDatas.Clear();
                var datas = pencilList.CreateDrawableData();
                for (int i = 0; i < datas.Count; i++)
                {
                    PencilData pencilData = new PencilData(pencilList.sprites[i].name, datas[i]);
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
                        scrollViewManager.pencilManager.SelectPencil(pencilList.pencil, pencilData.data);
                        scrollViewManager.ObjectViewOnOff(this);
                    });
                    children[i].GetComponentInChildren<Image>().sprite = pencilData.data.sprite;
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

