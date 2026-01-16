using System;
using UnityEngine;
using System.IO;
using MapEditor.Pencil;
using TMPro;
using UnityEngine.UI;

namespace MapEditor
{
    public class UISaveLoadManager : MonoBehaviour
    {
        public GameObject saveMenu;
        public TMP_InputField fileNameField;
        public GameObject loadFileScroll;
        public Transform fileScrollContent;
        public GameObject ScrollDataPrefab;

        public string[] files;
        private string path;

        private void Start()
        {
            path = Path.Combine(Application.persistentDataPath, "Json", "Map");
        }

        public void OnSaveMenu()
        {
            saveMenu.SetActive(true);
        }
        
        public void Save()
        {
            string fileName = fileNameField.text;
            if (fileName == string.Empty)
                return;
            
            GameManager.Instance.SaveMap(fileName);
            fileNameField.text = "";
            saveMenu.SetActive(false);
        }

        public void CloseSave()
        {
            fileNameField.text = "";
            saveMenu.SetActive(false);
        }

        public void OnLoadFileScroll()
        {
            int childCount = fileScrollContent.childCount;
            files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                fileName = fileName.Split(".json")[0];
                Button[] button;
                TextMeshProUGUI textMesh;
                
                if (i < childCount)
                {
                    var obj = fileScrollContent.GetChild(i);
                    button = obj.GetComponentsInChildren<Button>();
                    textMesh = obj.GetComponentInChildren<TextMeshProUGUI>();
                }
                else
                {
                    var obj = Instantiate(ScrollDataPrefab, Vector3.zero, Quaternion.identity, fileScrollContent);
                    button = obj.GetComponentsInChildren<Button>();
                    textMesh = obj.GetComponentInChildren<TextMeshProUGUI>();
                }
                
                textMesh.text = fileName;
                button[0].onClick.AddListener(CloseLoad);
                button[1].onClick.AddListener(() =>
                {
                    GameManager.Instance.LoadMap(fileName);
                    CloseLoad();
                });
            }
            loadFileScroll.SetActive(true);
        }

        public void Load()
        {

        }

        public void CloseLoad()
        {
            loadFileScroll.SetActive(false);
        }
    }
}
