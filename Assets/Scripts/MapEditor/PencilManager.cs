using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public static PencilManager Instance { get; private set; }

        public Image selectedPencilImage;
        public Drawable pencil;
        
        private Stack<List<GameObject>> undoStack;
        private Stack<List<GameObject>> redoStack;
        private List<GameObject> undoList;
        private bool isDrawReady;
        private bool isBegin;
        private bool isEnd;
        private float touchStartTime;
        private float drawStartTime;

        private void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            undoStack = new Stack<List<GameObject>>();
            redoStack = new Stack<List<GameObject>>();
            undoList = new List<GameObject>();
        }

        private void Start()
        {
            SelectPencil(pencil, pencil.data);
        }

        private void Update()
        {
            int touchCount = Touch.activeTouches.Count;
            if (touchCount == 0)
            {
                if (isEnd)
                {
                    pencil.End();
                    isEnd = false;
                }
                isDrawReady = true;
                isBegin = true;
                touchStartTime = 0f;
                
                if (undoList.Count > 0)
                {
                    undoStack.Push(undoList);
                    undoList = new List<GameObject>();
                }
            }
            else if (touchCount == 1)
            {
                if (touchStartTime < drawStartTime)
                {
                    touchStartTime += Time.deltaTime;
                    return;
                }
                
                if (isDrawReady)
                {
                    if (isBegin)
                    {
                        pencil.Begin();
                        isBegin = false;
                        isEnd = true;
                    }
                    
                    var obj = pencil.Draw(Touch.activeTouches[0].screenPosition);

                    if (obj != null)
                    {
                        var objTrans = obj.transform;
                        var penData = pencil.data;
                        objTrans.SetParent(GameManager.Instance.mapParent);
                        var data = new MapData(objTrans.position, penData.sprite, penData.defaultObj);
                        GameManager.Instance.mapList.dataList.Add(data);
                        
                        undoList.Add(obj);
                        if (redoStack.Count > 0)
                            redoStack.Clear();
                    }
                }
            }
            else
            {
                isDrawReady = false;
            }
        }

        public void Undo()
        {
            if (undoStack.Count <= 0)
                return;

            var list = undoStack.Peek();
            undoStack.Pop();
            redoStack.Push(list);
            
            // change action
            foreach (var obj in list)
            {
                obj.SetActive(false);
            }
        }

        public void Redo()
        {
            if (redoStack.Count <= 0)
                return;

            var list = redoStack.Pop();
            undoStack.Push(list);

            foreach (var obj in list)
            {
                obj.SetActive(true);
            }
        }

        public void SelectPencil(Drawable pen)
        {
            isDrawReady = false;
            pencil = pen;
            selectedPencilImage.sprite = pen.data.sprite;
        }
        
        public void SelectPencil(Drawable pen, DrawableData data)
        {
            SelectPencil(pen);
            pen.data = data.LoadData();
            selectedPencilImage.sprite = pen.data.sprite;
        }
    }
}
