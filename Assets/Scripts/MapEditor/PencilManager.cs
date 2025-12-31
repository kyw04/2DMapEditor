using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using TMPro;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public static PencilManager Instance { get; private set; }
        
        public TextMeshProUGUI testText;
        
        public Drawable pencil;
        
        private Stack<List<GameObject>> undoStack;
        private Stack<List<GameObject>> redoStack;
        private List<GameObject> undoList;
        private bool isDrawReady;
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
            SelectPencil(pencil);
        }

        private void Update()
        {
            int touchCount = Touch.activeTouches.Count;
            testText.text = touchCount.ToString();
            if (touchCount == 0)
            {
                pencil.End();
                isDrawReady = true;
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
                    var obj = pencil.Draw(Touch.activeTouches[0].screenPosition);

                    if (obj != null)
                    {
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
            pencil = pen;
        }
    }
}
