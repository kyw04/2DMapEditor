using MapEditor;
using UnityEngine;

public class RenderData : MonoBehaviour
{
    public bool isErased;
    public MapData mapData;
    private new SpriteRenderer renderer;
    
    public void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(GameObject worldObj, Vector2 pos, Sprite sprite, GameObject defaultObj)
    {
        isErased = false;
        mapData = new MapData(worldObj, pos, sprite, defaultObj);
    }

    public void ChangeMapData(MapData data)
    {
        if (mapData == data)
            return;
        
        if (data.isActivate)
            Activate();
        else
            Disable();

        mapData = data;
        renderer.sprite = data.sprite;
    }
    
    public void Activate()
    {
        isErased = false;
        mapData.isActivate = true;
        renderer.enabled = true;
        renderer.sprite = mapData.sprite;
    }

    public void Disable()
    {
        isErased = true;
        mapData.isActivate = false;
        renderer.enabled = false;
    }
}
