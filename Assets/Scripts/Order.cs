using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] middleRenderers;
    [SerializeField] TMP_SubMesh[] middleSubMeshes;
    [SerializeField] string sortingLayerName;
    int originOrder;

    public void SetOriginOrder(int originOrder) // 최초의 order 설정
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront, int order = 1) // isMostFront == true이면 가장 앞으로 가져옴 , false이면 원래 위치로 옮겨줌
    {
        if (isMostFront)
        {
            int mulOrder = order * 10;

            foreach (var renderer in backRenderers)
            {
                renderer.sortingLayerName = "MostFront";
                renderer.sortingOrder = mulOrder;
            }

            foreach (var renderer in middleRenderers)
            {
                renderer.sortingLayerName = "MostFront";
                renderer.sortingOrder = mulOrder + 1;
            }
        }
        else
        {
            SetOrder(originOrder);
        }
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        foreach (var renderer in backRenderers)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach (var renderer in middleRenderers)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
        }
    }
}
