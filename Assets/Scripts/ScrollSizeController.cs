using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class ScrollSizeController : MonoBehaviour
{
    private RectTransform target;
    protected float spacing;
    protected float bottom;

    protected virtual void Awake()
    {
        Debug.Log($"{gameObject.name}: test");
        target = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        SetHeight();
    }

    public void SetHeight()
    {
        float totalHeight = 0f;
        foreach (var t in transform.GetComponentsInChildren<RectTransform>(true))
        {
            if (t.transform.parent != transform)
                continue;

            totalHeight += t.rect.height + spacing;
        }
        totalHeight += bottom;
        
        target.sizeDelta = new Vector2(target.rect.x, totalHeight);
    }
}
