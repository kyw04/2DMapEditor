using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEditor.Pencil
{
    [Serializable]
    public class DrawableData
    {
        public GameObject defaultObj;
        public Sprite sprite;
        public string path { get; private set; }
        public string directoryPath { get; private set; }

        public DrawableData(Drawable pencil, string addPath)
        {
            DrawableData penData = pencil.data;
            defaultObj = penData.defaultObj;
            sprite = penData.sprite;
            
            directoryPath = Path.Combine(Application.persistentDataPath, "Json");
            path = Path.Combine(directoryPath, addPath + ".json");
        }
        
        public void SaveData()
        {
            if (!File.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            File.WriteAllText(path, JsonUtility.ToJson(this, true));
        }

        public DrawableData LoadData()
        {
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonUtility.FromJson<DrawableData>(jsonString);
            }
            
            Debug.LogWarning("DrawableData: could not find data path.");
            return this;
        }
    }
    
    public abstract class Drawable : ScriptableObject
    {
        public DrawableData data;
        public ContactFilter2D filter;
        private const int RayMaxCount = 10;
        
        public virtual RenderData Draw(Vector2 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return null;
            
            pos = Camera.main.ScreenToWorldPoint(pos);
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y, 0);
            
            RaycastHit2D[] results = new RaycastHit2D[RayMaxCount];
            int hitCount = Physics2D.Raycast(pos, Vector3.forward, filter, results);
            RenderData renderData = null;
            for (int i = 0; i < hitCount; i++)
            {
                RenderData temp = results[i].transform.GetComponent<RenderData>();
                if (temp == null)
                    continue;

                if (!temp.isErased)
                {
                    renderData = temp;
                    break;
                }
            }

            int sortingOrder = 0;
            for (int i = 1; i <= 2; i++)
            {
                GameManager.Instance.mapRenders.TryGetValue(new Vector2(x + Mathf.Pow(-1, i), y), out var renderDataList);
                sortingOrder = Mathf.Max(sortingOrder, MaxSortingOrder(renderDataList));
                GameManager.Instance.mapRenders.TryGetValue(new Vector2(x, y + Mathf.Pow(-1, i)), out renderDataList);
                sortingOrder = Mathf.Max(sortingOrder, MaxSortingOrder(renderDataList));
            }
            
            if (renderData == null)
                return CreateRender(sortingOrder, pos);

            renderData.renderer.sortingOrder = sortingOrder;
            return ChangeRender(renderData, pos);
        }

        private int MaxSortingOrder(List<RenderData> renders)
        {
            if (renders == null)
                return 0;
            
            int max = 0;
            foreach (var render in renders)
            {
                max = Mathf.Max(max, render.renderer.sortingOrder);
            }
            
            return max + 1;
        }

        public abstract void Begin();
        public abstract void End();
        protected abstract RenderData CreateRender(int sortingOrder, Vector2 pos);
        protected abstract RenderData ChangeRender(RenderData renderData, Vector2 pos);
    }
}
