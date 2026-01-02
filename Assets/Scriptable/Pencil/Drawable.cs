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
        public bool isFloor;
        private string path;
        private string directoryPath;
        
        public DrawableData(GameObject obj, Sprite sp, bool bo, string addPath)
        {
            defaultObj = obj;
            sprite = sp;
            isFloor = bo;

            
            directoryPath = Path.Combine(Application.persistentDataPath, "Json");
            path = Path.Combine(directoryPath, addPath + ".json");
        }

        public DrawableData(Drawable pencil, string addPath)
        {
            DrawableData penData = pencil.data;
            defaultObj = penData.defaultObj;
            sprite = penData.sprite;
            isFloor = penData.isFloor;
            
            directoryPath = Path.Combine(Application.persistentDataPath, "Json");
            path = Path.Combine(directoryPath, addPath + ".json");
        }
        
        public void SaveData()
        {
            if (!File.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            File.WriteAllText(path, JsonUtility.ToJson(this, true));
            Debug.Log($"DrawableData: data saved -> {path}");
        }

        public DrawableData LoadData()
        {
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonUtility.FromJson<DrawableData>(jsonString);
            }
            
            Debug.LogWarning("DrawableData: could not find data path.");
            return null;
        }
    }
    
    public abstract class Drawable : ScriptableObject
    {
        public DrawableData data;

        public Sprite GetSprite() { return data.sprite; }
        
        public virtual GameObject Draw(Vector3 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return null;
            
            pos = Camera.main.ScreenToWorldPoint(pos);
            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            pos = new Vector3(x, y, 0);
            
            if (!data.isFloor && Physics2D.Raycast(pos, Vector3.forward))
                return ChangeRender(pos);
            
            return CreateRender(pos);
        }

        public virtual void Begin()
        {
            data = data.LoadData();
        }

        public abstract void End();
        protected abstract GameObject CreateRender(Vector3 pos);
        protected abstract GameObject ChangeRender(Vector3 pos);
    }
}
