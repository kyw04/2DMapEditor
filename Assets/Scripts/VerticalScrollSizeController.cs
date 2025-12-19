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
        spacing = layoutGroup.spacing;
        bottom = layoutGroup.padding.bottom;
    }
}
