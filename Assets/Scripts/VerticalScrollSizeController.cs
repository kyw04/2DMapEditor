using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalScrollSizeController : ScrollSizeController
{
    private VerticalLayoutGroup layoutGroup;
    
    protected override void Awake()
    {
        base.Awake();
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        spacingY = layoutGroup.spacing;
        top = layoutGroup.padding.top;
        bottom = layoutGroup.padding.bottom;
    }
}
