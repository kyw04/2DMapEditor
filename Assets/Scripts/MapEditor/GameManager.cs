using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
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

        public MapData Clone()
        {
            var c = new MapData(worldObj, new Vector2(x, y), sprite, defaultObj);
            c.isActivate = isActivate;
            return c;
        }

        public bool Compare(MapData other)
        {
            return  worldObj == other.worldObj &&
                    sprite == other.sprite &&
                    defaultObj == other.defaultObj;
        }
    }
    
    [Serializable]
    public class MapList
    {
        public List<MapData> dataList = new();
    }

    public struct MapChange
    {
        public RenderData target;
        public MapData before;
        public MapData after;
    }

    public class UndoList
    {
        public int Count => changes.Count;
        private List<MapChange> changes = new();

        public void Add(RenderData target, MapData before, MapData after)
        {
            if (target == null)
                return;

            changes.Add(new MapChange
            {
                target = target,
                before = before.Clone(),
                after = after.Clone()
            });
        }

        public MapChange Get(int index) => changes[index];
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public MapList mapList;
        public Transform mapParent;
        public GameObject mapSaveBackground;
        
        private string directoryPath;
        private Coroutine coroutine;
        
        public Dictionary<Vector2, List<RenderData>> mapRenders { get; private set; }
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
                var ch = list.Get(i);
                ch.target.Apply(ch.after);
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
                var ch = list.Get(i);
                ch.target.Apply(ch.before);
            }

            yield return new WaitForEndOfFrame();
            coroutine = null;
        }

        public RenderData FindRenderData(Vector2 pos, MapData data)
        {
            if (mapRenders.TryGetValue(pos, out var renders))
            {
                foreach (var render in renders)
                {
                    if (data.Compare(render.mapData))
                    {
                        return render;
                    }
                }
            }
            
            return null;
        }
        
        public void DeleteMap(RenderData renderData)
        {
            renderData.Disable();
        }

        public RenderData ChangeMap(RenderData renderData, MapData mapData)
        {
            Vector2 pos = new Vector2(mapData.x, mapData.y);
            var render = FindRenderData(pos, renderData.mapData);

            if (!mapData.Compare(renderData.mapData))
            {
                render.ChangeMapData(mapData);
                return render;
            }

            return null;
        }
        
        public RenderData CreateMap(MapData data, bool isAddDataList = true)
        {
            Vector2 pos = new Vector2(data.x, data.y);
            var render = FindRenderData(pos, data);
            if (render != null)
                return render;
            
            var obj = Instantiate(data.defaultObj, pos, Quaternion.identity, mapParent);
            data.worldObj = obj;

            RenderData newRenderData = obj.GetComponent<RenderData>();
            newRenderData ??= obj.AddComponent<RenderData>();
            newRenderData.SetData(obj, pos, data.sprite, data.defaultObj);
            newRenderData.Activate(recordOld: false);
            
            mapRenders.TryAdd(pos, new List<RenderData>());
            mapRenders[pos].Add(newRenderData);
            
            if (isAddDataList)
                mapList.dataList.Add(data);
            
            return newRenderData;
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
                var before = data.Clone();
                data.sprite = null;
                tempRenderDataList.Add(CreateMap(data, false), before, data);
            }
            undoStack.Push(tempRenderDataList);
            
            yield return new WaitForEndOfFrame();
            
            coroutine = null;
            mapSaveBackground.SetActive(false);
        }
        
        public void SaveMap(string fileName)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            mapList = BuildMapListFromRenders();

            coroutine ??= StartCoroutine(_StartSaveMap(fileName));
        }

        private MapList BuildMapListFromRenders()
        {
            var list = new MapList();

            foreach (var kv in mapRenders)
            {
                var renders = kv.Value;
                if (renders == null) continue;

                foreach (var r in renders)
                {
                    if (r == null || r.mapData == null) continue;
                    if (!r.mapData.isActivate) continue;
                    if (r.mapData.defaultObj == null) continue;

                    Vector2 pos = new Vector2(r.mapData.x, r.mapData.y);
                    var data = new MapData(null, pos, r.mapData.sprite, r.mapData.defaultObj)
                    {
                        isActivate = true
                    };

                    list.dataList.Add(data);
                }
            }

            return list;
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
