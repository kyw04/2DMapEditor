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
        public float drawStartTime;

        private List<RenderData> undoList;
        private bool isDrawReady;
        private bool isBegin;
        private bool isEnd;
        private float touchStartTime;

        private void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            undoList = new List<RenderData>();
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
                    GameManager.Instance.undoStack.Push(undoList);
                    undoList = new List<RenderData>();
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
                        objTrans.SetParent(GameManager.Instance.mapParent);
                        undoList.Add(GameManager.Instance.CreateMap(obj));
                        if (GameManager.Instance.redoStack.Count > 0)
                            GameManager.Instance.redoStack.Clear();
                    }
                }
            }
            else
            {
                isDrawReady = false;
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
