using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace MapEditor
{
    [Serializable]
    public class MapData
    {
        public bool isActivate;
        public GameObject worldObj;
        public float x, y;
        public Sprite sprite;
        public GameObject defaultObj;

        public MapData(GameObject worldObj, Vector2 pos, Sprite sprite, GameObject defaultObj)
        {
            isActivate = false;
            this.worldObj = worldObj;
            this.x = pos.x;
            this.y = pos.y;
            this.sprite = sprite;
            this.defaultObj = defaultObj;
        }
    }
    
    [Serializable]
    public class MapList
    {
        public List<MapData> dataList = new();
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public MapList mapList;
        public Transform mapParent;
        public GameObject mapSaveBackground;
        
        private string directoryPath;
        private Coroutine coroutine;
        private Dictionary<Vector2, List<RenderData>> mapRenders;
        
        public Stack<List<RenderData>> undoStack { get; private set; }
        public Stack<List<RenderData>> redoStack { get; private set; }
        private List<RenderData> tempRenderDataList;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnhancedTouchSupport.Enable();

            directoryPath = Path.Combine(Application.persistentDataPath, "Json", "Map");
            mapList = new MapList();
            
            undoStack = new Stack<List<RenderData>>();
            redoStack = new Stack<List<RenderData>>();
            mapRenders = new Dictionary<Vector2, List<RenderData>>();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }
        
        public void Undo()
        {
            if (undoStack.Count <= 0)
                return;
            
            coroutine ??= StartCoroutine(_StartUndo());
        }

        private IEnumerator _StartUndo()
        {
            var list = undoStack.Pop();
            redoStack.Push(list);

            if (list[0].isErased)
            {
                foreach (var data in list)
                {
                    CreateMap(data.mapData);
                }
            }
            else
            {
                DeleteMap(list);
            }
            
            yield return new WaitForEndOfFrame();
            coroutine = null;
        }

        public void Redo()
        {
            if (redoStack.Count <= 0)
                return;
            
            coroutine ??= StartCoroutine(_StartRedo());
        }

        private IEnumerator _StartRedo()
        {
            var list = redoStack.Pop();
            undoStack.Push(list);

            if (list[0].isErased)
            {
                foreach (var data in list)
                {
                    CreateMap(data);
                }
            }
            else
            {
                DeleteMap(list);
            }

            yield return new WaitForEndOfFrame();
            coroutine = null;
        }

        public void DeleteMap(RenderData renderData)
        {
            MapData mapData = renderData.mapData;
            Vector2 pos = new Vector2(mapData.x, mapData.y);
            mapRenders[pos].Remove(renderData);
            renderData.Disable();
        }
        
        public void DeleteMap(List<RenderData> list)
        {
            foreach (var data in list)
            {
                DeleteMap(data);
            }
        }

        public RenderData ChangeMap(RenderData renderData, MapData mapData)
        {
            renderData.mapData = mapData;
            renderData.ChangeSprite(renderData.mapData.sprite);
            renderData.Activate();

            return renderData;
        }
        
        public RenderData CreateMap(MapData data, bool isAddDataList = true)
        {
            Vector2 pos = new Vector2(data.x, data.y);
            if (mapRenders.TryGetValue(pos, out var renders))
            {
                foreach (var render in renders)
                {
                    if (data.sprite == render.mapData.sprite)
                    {
                        return render;
                    }
                }
            }
            
            var obj = Instantiate(data.defaultObj, pos, Quaternion.identity, mapParent);
            data.worldObj = obj;

            RenderData newRenderData = obj.GetComponent<RenderData>();
            newRenderData ??= obj.AddComponent<RenderData>();
            newRenderData.SetData(obj,pos, data.sprite, data.defaultObj);
            newRenderData.Activate();
            
            mapRenders.TryAdd(pos, new List<RenderData>());
            mapRenders[pos].Add(newRenderData);
            
            if (isAddDataList)
                mapList.dataList.Add(data);
            
            return newRenderData;
        }

        public RenderData CreateMap(RenderData data, bool isAddDataList = true)
        {
            if (data.mapData.worldObj != null)
            {
                data.Activate();
                return data;
            }

            return CreateMap(data.mapData);
        }

        public void LoadMap(string fileName)
        {
            string path = Path.Combine(directoryPath, fileName + ".json");
            if (!File.Exists(path))
            {
                return;
            }
            
            string jsonString = File.ReadAllText(path);
            mapList = JsonUtility.FromJson<MapList>(jsonString);
            coroutine ??= StartCoroutine(_StartLoadMap());
        }

        private IEnumerator _StartLoadMap()
        {
            tempRenderDataList = new List<RenderData>();
            mapSaveBackground.SetActive(true);
            foreach (var data in mapList.dataList)
            {
                tempRenderDataList.Add(CreateMap(data, false));
            }
            undoStack.Push(tempRenderDataList);
            
            yield return new WaitForEndOfFrame();
            
            coroutine = null;
            mapSaveBackground.SetActive(false);
        }
        
        public void SaveMap(string fileName)
        {
            if (!File.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            coroutine ??= StartCoroutine(_StartSaveMap(fileName));
        }

        private IEnumerator _StartSaveMap(string fileName)
        {
            mapSaveBackground.SetActive(true);
            File.WriteAllText(Path.Combine(directoryPath, fileName + ".json"), JsonUtility.ToJson(mapList, true));
            yield return new WaitForEndOfFrame();
            
            coroutine = null;
            mapSaveBackground.SetActive(false);
            Debug.Log($"GameManager: save map to json {Path.Combine(directoryPath, fileName + ".json")}");
        }
    }
}
