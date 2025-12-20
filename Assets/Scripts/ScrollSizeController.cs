using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class ScrollSizeController : MonoBehaviour
{
    private RectTransform target;
    protected float spacingX;
    protected float spacingY;
    protected float top;
    protected float bottom;

    private bool onFirst;

    protected virtual void Awake()
    {
        target = GetComponent<RectTransform>();
    }

    private void Start()
    {
        SetHeight();
    }

    public void SetHeight()
    {
        var children = transform.GetComponentsInChildren<RectTransform>(true);
        float totalHeight = 0f, childrenWidth = 0f;
        int widthCount = 0, stack = 0;
        onFirst = true;
        
        foreach (var t in children)
        {
            if (t.transform.parent != transform)
                continue;
            childrenWidth = t.rect.width;

            if (onFirst)
            {
                onFirst = false;
                stack = (int)(children[0].rect.width / (childrenWidth + spacingX));
                if (stack <= 0)
                    stack = 1;
            }

            if (stack <= ++widthCount)
            {
                widthCount = 0;
                totalHeight += t.rect.height + spacingY;
            }
        }

        if (0 < widthCount)
            totalHeight += childrenWidth;
        totalHeight += bottom + top;
        
        target.sizeDelta = new Vector2(target.rect.x, totalHeight);
    }
}
