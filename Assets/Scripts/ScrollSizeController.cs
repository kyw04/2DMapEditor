using System.Collections;
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
    private Coroutine coroutine;
    
    protected virtual void Awake()
    {
        target = GetComponent<RectTransform>();
    }

    private void Start()
    {
        coroutine = null;
        SetHeight();
    }

    private IEnumerator StartSetHeight()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        
        var children = transform.GetComponentsInChildren<RectTransform>();
        float totalHeight = 0f, childWidth = 0f;
        int widthCount = 0, stack = 0;
        onFirst = true;
        
        foreach (var t in children)
        {
            if (t.transform.parent != transform)
                continue;
            childWidth = t.rect.width;

            if (onFirst)
            {
                onFirst = false;
                stack = (int)(children[0].rect.width / (childWidth + spacingX));
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
            totalHeight += childWidth;
        totalHeight += bottom + top;
        
        target.sizeDelta = new Vector2(target.rect.x, totalHeight);
        coroutine = null;
    }
    
    public void SetHeight()
    {
        coroutine ??= StartCoroutine(StartSetHeight());
    }
}
