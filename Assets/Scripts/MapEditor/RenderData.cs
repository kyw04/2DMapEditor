using MapEditor;
using UnityEngine;

public class RenderData : MonoBehaviour
{
    public bool isErased;
    public MapData oldMapData;
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
        oldMapData = new MapData(null, pos, null, defaultObj);
    }
    
    public void Apply(MapData data)
    {
        mapData = data;

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
        if (mapData.Compare(data))
            return;

        oldMapData = mapData;
        mapData = data;
        renderer.sprite = data.sprite;
        
        if (data.isActivate)
            Activate();
        else
            Disable();
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
        oldMapData = mapData;
        isErased = true;
        mapData.isActivate = false;
        renderer.enabled = false;
    }
}
