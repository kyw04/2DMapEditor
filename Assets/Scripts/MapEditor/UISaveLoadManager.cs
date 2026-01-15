using System;
using UnityEngine;
using System.IO;
using MapEditor.Pencil;
using TMPro;

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
                TextMeshProUGUI textMesh;
                if (i < childCount)
                {
                    var obj = Instantiate(ScrollDataPrefab, Vector3.zero, Quaternion.identity, fileScrollContent);
                    textMesh = obj.GetComponentInChildren<TextMeshProUGUI>();
                }
                else
                {
                    textMesh = fileScrollContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                    Debug.Log(fileScrollContent.GetChild(i).name);
                }

                textMesh.text = files[i];
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
