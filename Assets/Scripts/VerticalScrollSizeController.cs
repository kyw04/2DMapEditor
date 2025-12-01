using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalScrollSizeController : MonoBehaviour
{
    private RectTransform target;
    private VerticalLayoutGroup layoutGroup;
    private float spacing;
    
    private void Awake()
    {
        target = GetComponent<RectTransform>();
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        spacing = layoutGroup.spacing;
    }

    private void Start()
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

        target.sizeDelta = new Vector2(target.rect.x, totalHeight);
    }
}
