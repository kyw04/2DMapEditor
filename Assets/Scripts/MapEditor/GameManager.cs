using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace MapEditor
{
    [Serializable]
    public class MapData
    {
        public float x, y;
        public Sprite sprite;
        public GameObject defaultObj;

        public MapData(Vector2 pos, Sprite sprite, GameObject defaultObj)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.sprite = sprite;
            this.defaultObj = defaultObj;
        }
    }
    [Serializable]
    public class MapList
    {
        public List<MapData> dataList;

        public MapList()
        {
            dataList = new List<MapData>();
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
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        public void DeleteMap()
        {
            
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
            foreach (var data in mapList.dataList)
            {
                 var render = Instantiate(data.defaultObj, new Vector3(data.x, data.y), quaternion.identity).GetComponent<SpriteRenderer>();
                 render.sprite = data.sprite;
            }
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
