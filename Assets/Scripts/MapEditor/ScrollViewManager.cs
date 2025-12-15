using MapEditor.Pencil;
using UnityEngine;
using TMPro;

namespace MapEditor
{
    public class ScrollViewManager : MonoBehaviour
    {
        public PencilManager pencilManager;
        public GameObject backgroundShield;
        public GameObject headerView;
        public GameObject objectView;
        public Transform objectViewContent;

        private VerticalScrollSizeController sizeController;
        private ScrollData currentScroll;

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

        public void ObjectViewOnOff(ScrollData data)
        {
            if (currentScroll == data)
            {
                objectView.SetActive(!objectView.activeSelf);
            }
            else
            {
                data.ButtonSetting(this);
                sizeController.SetHeight();
                objectView.SetActive(true);
            }

            currentScroll = data;
        }
    }
}
