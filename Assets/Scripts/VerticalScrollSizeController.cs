using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalScrollSizeController : MonoBehaviour
{
    private RectTransform target;
    private VerticalLayoutGroup layoutGroup;
    private float totalHeight;
    
    private void Start()
    {
        target = GetComponent<RectTransform>();
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        totalHeight = 0f;
        foreach (var t in transform.GetComponentsInChildren<RectTransform>())
        {
            if (t.transform.parent != transform)
                continue;
            
            totalHeight += t.rect.height + layoutGroup.spacing;
        }

        target.sizeDelta = new Vector2(target.rect.x, totalHeight);
    }
}
