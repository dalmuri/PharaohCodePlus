using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyOperand : OperatorOrOperand
{
    [SerializeField] Renderer hoverBox;
    [SerializeField] BoxCollider2D boxCollider2D;

    public bool visible { get; private set; }

    public void SetThisVisible(bool visible)
    {
        this.visible = visible;
        this.GetComponent<Renderer>().enabled = visible;

        if (visible)
            DragManager.Dragging += CheckHoverBoxVisible;
        else
        {
            DragManager.Dragging -= CheckHoverBoxVisible;
            hoverBox.enabled = false;
        }
    }

    public void CheckHoverBoxVisible(Vector3 touchPos, IDraggableObject draggableObject)
    {
        if (this.visible && !isDragging && draggableObject is OperatorOrOperand && boxCollider2D.bounds.Contains(touchPos))
        {
            hoverBox.enabled = true;
        }
        else
        {
            hoverBox.enabled = false;
        }
    }

    protected override void SetUpIntialState()
    {
        canDrag = false;
        width = widthOperandLength;
        height = heightOperandLength;

        SetThisVisible(true);
    }

    protected override void SetSizeOfThis()
    {
        // Do Nothing!
    }

    protected override void AlignPosOfExpression()
    {
        // Do Nothing!
    }

    public override int Calculate()
    {
        CalculateManager.instance.existsError[(int)ErrorType.Empty] = true;
        return errorNum;
    }

    private void OnDestroy()
    {
        DragManager.Dragging -= CheckHoverBoxVisible;
    }
}
