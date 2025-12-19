using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridScrollSizeController : ScrollSizeController
{
    private GridLayoutGroup layoutGroup;

    protected override void Awake()
    {
        base.Awake();
        layoutGroup = GetComponent<GridLayoutGroup>();
        spacing = layoutGroup.spacing.y;
        bottom = layoutGroup.padding.bottom;
    }
}