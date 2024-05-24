using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class NumberSquare : MonoBehaviour, IDraggableObject
{
    [SerializeField] SpriteRenderer numberSquareRenderer;
    [SerializeField] TMP_Text numberTMP;

    [SerializeField] Sprite spriteYellow;
    [SerializeField] Sprite spriteBlue;
    [SerializeField] Sprite spriteRed;
    [SerializeField] Sprite spritePurple;
    
    [SerializeField] Sprite spriteEmpty;

    public bool canDrag { get; set; }

    public NumberInfo numberInfo { get; private set; }
    public SquareColor squareColor { get; private set; }
    public int number { get; private set; }
    public int point { get; private set; }
    public bool isDragging { get; private set; } = false;

    public int indexOfFloor;

    public Vector3 originPos;

    public void SetUp(NumberInfo numberInfo, bool canDrag)
    {
        this.numberInfo = numberInfo;

        this.squareColor = numberInfo.squareColor;
        this.number = numberInfo.number;
        this.point = numberInfo.point;

        numberTMP.text = number.ToString();
        switch (squareColor)
        {
            case SquareColor.Yellow:
                numberSquareRenderer.sprite = spriteYellow;
                break;
            case SquareColor.Blue:
                numberSquareRenderer.sprite = spriteBlue;
                break;
            case SquareColor.Red:
                numberSquareRenderer.sprite = spriteRed;
                break;
            case SquareColor.Purple:
                numberSquareRenderer.sprite = spritePurple;
                break;
            case SquareColor.Empty:
                numberSquareRenderer.sprite = spriteEmpty;
                break;
        }

        this.GetComponent<Order>().SetOriginOrder(2);

        this.canDrag = canDrag;
    }

    public void MoveTransform(Vector3 pos, bool usesDotween, float dotweenTime = 0)
    {
        if (usesDotween)
        {
            transform.DOMove(pos, dotweenTime);
            // transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = pos;
        }
    }

    bool ExistsInHandArea(Vector3 touchPos)
    {
        touchPos.z = -10;
        RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, Vector3.forward);
        int layer = LayerMask.NameToLayer("HandArea");
        return Array.Exists(hits, x => x.collider.gameObject.layer == layer); // hits 중 HandArea가 있으면 return true
    }

    public void DragStart(Vector3 touchPos)
    {
        isDragging = true;

        this.GetComponent<Order>().SetMostFrontOrder(true);
    }

    public void Drag(Vector3 touchPos)
    {
        MoveTransform(touchPos, false);
        HandManager.instance.CheckHoverBoxVisible(ExistsInHandArea(touchPos));
    }

    public void DragEnd(Vector3 touchPos)
    {
        isDragging = false;
        HandManager.instance.CheckHoverBoxVisible(false);

        if (ExistsInHandArea(touchPos)) // HandArea에서 드래그를 끝낸 경우
        {
            HandManager.instance.AddHand(this.numberInfo);
            int floor;
            switch (squareColor)
            {
                case SquareColor.Yellow:
                    floor = 0;
                    break;
                case SquareColor.Blue:
                    floor = 1;
                    break;
                case SquareColor.Red:
                    floor = 2;
                    break;
                case SquareColor.Purple:
                    floor = 3;
                    break;
                case SquareColor.Empty:
                default:
                    floor = -1;
                    break;
            }

            NumberSquareManager.instance.EmptyOutNumberSquare(floor, indexOfFloor);
        }
        else
        {
            MoveTransform(originPos, true, 0.2f);
            this.GetComponent<Order>().SetMostFrontOrder(false);
        }
    }
}
