using System;
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
        
        
        public virtual RenderData Draw(Vector2 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return null;
            
            pos = Camera.main.ScreenToWorldPoint(pos);
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y, 0);
                
            RaycastHit2D hitData = Physics2D.Raycast(pos, Vector3.forward);
            if (hitData)
            {
                RenderData renderData = hitData.transform.GetComponent<RenderData>();

                if (data.defaultObj == null)
                    return ChangeRender(renderData, pos);

                if (renderData == null || renderData.mapData == null ||
                    !renderData.CompareTag(data.defaultObj.tag))
                {
                    return CreateRender(pos);
                }
                
                return ChangeRender(renderData, pos);
            }
            
            return CreateRender(pos);
        }

        public abstract void Begin();
        public abstract void End();
        protected abstract RenderData CreateRender(Vector2 pos);
        protected abstract RenderData ChangeRender(RenderData renderData, Vector2 pos);
    }
}
