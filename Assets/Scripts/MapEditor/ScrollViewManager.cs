using UnityEngine;
using TMPro;

namespace MapEditor
{
    public class ScrollViewManager : MonoBehaviour
    {
        public GameObject headerView;
        public GameObject objectView;
        public TextMeshProUGUI headerTxt;

        private VerticalScrollSizeController sizeController;

        private string currentHeader;
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

        public void ObjectViewOnOff(string headerID)
        {
            if (currentHeader == headerID)
            {
                ScrollViewOnOff(objectView);
            }
            else
            {
                isSelectHeader = objectView.activeSelf;
                sizeController.SetHeight();
                if (!isSelectHeader)
                    ScrollViewOnOff(objectView);
            }
            
        }
        
        private void ScrollViewOnOff(GameObject target)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
