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
            isActivate = true;
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

    public class UndoList
    {
        public int Count => renderData.Count;
        
        private List<RenderData> renderData = new();
        private List<MapData> returnMapData = new();

        public void Push(RenderData renderValue)
        {
            renderData.Add(renderValue);
            returnMapData.Add(renderValue.oldMapData);
        }

        public KeyValuePair<RenderData, MapData> GetValueToIndex(int index)
        {
            return new KeyValuePair<RenderData, MapData>(renderData[index], returnMapData[index]);
        }
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
        
        public Stack<UndoList> undoStack { get; private set; }
        public Stack<UndoList> redoStack { get; private set; }
        private UndoList tempRenderDataList;

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
            
            undoStack = new Stack<UndoList>();
            redoStack = new Stack<UndoList>();
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
            UndoList list = undoStack.Pop();
            redoStack.Push(list);

            for (int i = 0; i < list.Count; i++)
            {
                var data = list.GetValueToIndex(i);
                ChangeMap(data.Key, data.Value);
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
            UndoList list = redoStack.Pop();
            undoStack.Push(list);

            for (int i = 0; i < list.Count; i++)
            {
                var data = list.GetValueToIndex(i);
                ChangeMap(data.Key, data.Value);
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

        public RenderData ChangeMap(RenderData renderData, MapData mapData)
        {
            Vector2 pos = new Vector2(mapData.x, mapData.y);
            
            if (mapRenders.TryGetValue(pos, out var renders))
            {
                foreach (var render in renders)
                {
                    if (renderData.CompareTag(render.tag))
                    {
                        render.ChangeMapData(mapData);
                        return render;
                    }
                }
            }

            return null;
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
            mapSaveBackground.SetActive(true);
            
            tempRenderDataList = new UndoList();
            foreach (var data in mapList.dataList)
            {
                tempRenderDataList.Push(CreateMap(data, false));
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
