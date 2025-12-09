using UnityEngine;
using TMPro;

namespace MapEditor
{
    public class ScrollViewManager : MonoBehaviour
    {
        public GameObject backgroundShield;
        public GameObject headerView;
        public GameObject objectView;

        private VerticalScrollSizeController sizeController;
        private string currentHeader;

        private void Start()
        {
            backgroundShield.SetActive(false);
            headerView.SetActive(false);
            objectView.SetActive(false);
            sizeController = objectView.GetComponentInChildren<VerticalScrollSizeController>();
        }

        public void HeaderViewOnOff()
        {
            headerView.SetActive(!headerView.activeSelf);
            backgroundShield.SetActive(headerView.activeSelf);
            if (!headerView.activeSelf)
            {
                objectView.SetActive(false);
            }
        }

        public void ObjectViewOnOff(string headerID)
        {
            if (currentHeader == headerID)
            {
                objectView.SetActive(!objectView.activeSelf);
            }
            else
            {
                sizeController.SetHeight();
                objectView.SetActive(true);
            }

            currentHeader = headerID;
        }

        public void Test(ScrollData data)
        {
            Debug.Log(data.headerID);
        }
    }
}
