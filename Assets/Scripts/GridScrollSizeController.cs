using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridScrollSizeController : ScrollSizeController
{
    private GridLayoutGroup layoutGroup;
    private ScrollRect scrollRect;

    protected override void Awake()
    {
        base.Awake();
        layoutGroup = GetComponent<GridLayoutGroup>();
        spacingX = layoutGroup.spacing.x;
        spacingY = layoutGroup.spacing.y;
        top = layoutGroup.padding.top;
        bottom = layoutGroup.padding.bottom;
    }
}