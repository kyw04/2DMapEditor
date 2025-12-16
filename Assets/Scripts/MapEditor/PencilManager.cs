using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MapEditor.Pencil
{
    public class PencilManager : MonoBehaviour
    {
        public Drawable pencil;
        
        private Stack<List<GameObject>> undoStack;
        private Stack<List<GameObject>> redoStack;
        private List<GameObject> undoList;

        private void Awake()
        {
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
            if (Touch.activeTouches.Count > 0)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Touch.activeTouches[0].screenPosition);
                var obj = pencil.Draw(pos);

                if (obj != null)
                {
                    undoList.Add(obj);
                    if (redoStack.Count > 0)
                        redoStack.Clear();
                }
            }
            else if (undoList.Count > 0)
            {
                undoStack.Push(undoList);
                undoList = new List<GameObject>();
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
