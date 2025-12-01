using UnityEngine;

namespace MapEditor
{
    public class ScrollViewManager : MonoBehaviour
    {
        public GameObject headerView;
        public GameObject objectView;

        private VerticalScrollSizeController sizeController;
        
        private bool isSelectHeader;

        private void Start()
        {
            headerView.SetActive(false);
            objectView.SetActive(false);
            sizeController = objectView.GetComponentInChildren<VerticalScrollSizeController>();
            isSelectHeader = false;
        }

        public void HeaderViewOnOff()
        {
            ScrollViewOnOff(headerView);
            if (!headerView.activeSelf && objectView.activeSelf)
                ScrollViewOnOff(objectView);
        }

        public void ObjectViewOnOff()
        {
            ScrollViewOnOff(objectView);
            sizeController.SetHeight();
        }
        
        private void ScrollViewOnOff(GameObject target)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
