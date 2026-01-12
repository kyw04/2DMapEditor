using UnityEngine;

namespace MapEditor
{
    public class RenderData : MonoBehaviour
    {
        public bool isErased;
        public MapData oldMapData;
        public MapData mapData;
        public new SpriteRenderer renderer;
    
        public void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void SetData(GameObject worldObj, Vector2 pos, Sprite sprite, GameObject defaultObj, int sortingOrder = 0)
        {
            isErased = false;
            renderer.sortingOrder = sortingOrder;
            mapData = new MapData(worldObj, pos, sprite, defaultObj, sortingOrder);
            oldMapData = new MapData(worldObj, pos, null, defaultObj, sortingOrder) { isActivate = false };
        }
    
        public void Apply(MapData data)
        {
            mapData = data.Clone();

            if (mapData.isActivate)
            {
                isErased = false;
                renderer.enabled = true;
                renderer.sprite = mapData.sprite;
            }
            else
            {
                isErased = true;
                renderer.enabled = false;
                renderer.sprite = mapData.sprite;
            }
        }

        public void ChangeMapData(MapData data)
        {
            if (mapData.Compare(data) && mapData.isActivate == data.isActivate)
                return;

            oldMapData = mapData != null ? mapData.Clone() : null;
            mapData = data;
            renderer.sprite = data.sprite;

            if (data.isActivate)
                Activate(recordOld: false);
            else
                Disable(recordOld: false);
        }
    
        public void Activate(bool recordOld = true)
        {
            if (recordOld)
                oldMapData = mapData.Clone();
        
            isErased = false;
            mapData.isActivate = true;
            renderer.enabled = true;
            renderer.sprite = mapData.sprite;
        }

        public void Disable(bool recordOld = true)
        {
            if (recordOld)
                oldMapData = mapData.Clone();
        
            isErased = true;
            mapData.isActivate = false;
            renderer.enabled = false;
        }
    }
}
