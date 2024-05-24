using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public enum OperType { Operand, Operator, EqualSign } // +-×÷^√!
public abstract class OperatorOrOperand : MonoBehaviour, IDraggableObject
{
    [SerializeField] public OperType operType;
    [SerializeField] protected TMP_Text operTMP;
    [SerializeField] protected GameObject parenthesisGO1;
    [SerializeField] protected GameObject parenthesisGO2;

    /*
     * ArithmeticOpeartor Width
     * |-------||---0---||---+---||---0---||-------|
     *   0.325    child     0.8     child    0.325
     *           Empty=0.9         Empty=0.9
     * |-(-||---------------------------------||-)-|
     *  0.1    child/2 + 0.4 + 0.4 + child/2    0.1
     */

    protected const float heightInterval = 0.2f;
    protected const float heightOperandLength = 0.95f;
    protected const float heightMinimumIntervalBetweenChildren = 0.15f;

    protected const float widthOperandLength = 0.9f;
    protected const float widthBinaryOperator = 0.8f;
    protected const float widthIntervalBetweenExponentiationOperatorChildren = 0.1f;
    protected const float widthIntervalBetweenUnaryOperatorAndChild = 0.45f;
    protected const float widthIntervalBetweenParenthesisAndEdge = 0.1f;
    protected const float widthIntervalBetweenChildAndParenthesis = 0.225f;
    protected const float widthIntervalBetweenChildAndEdge = 0.325f;

    protected const float degreeOfTranslucence = 0.6f;

    protected const int errorNum = 0;

    [SerializeField] public float width = widthOperandLength;
    [SerializeField] public float height = heightOperandLength;

    [SerializeField] public EmptyOperand[] emptyOperands;

    [SerializeField] public OperatorOrOperand parentOperator = null;
    [SerializeField] public int childNum = 0; // 본인이 부모 객체의 몇 번째 child인지
    [SerializeField] public OperatorOrOperand[] childrenOpers;

    [SerializeField] public int order = 1; // sortingOrder

    public bool isUntouched { get; set; } = true;
    public bool canDrag { get; set; } = true;
    public bool isDragging = false;

    /// <summary>
    /// 최초의 상태를 설정. Start()에서 호출.
    /// </summary>
    protected abstract void SetUpIntialState();

    private void SetParenthesesVisible(bool visible)
    {
        if (operType == OperType.Operand || operType == OperType.EqualSign)
            return;
        parenthesisGO1.GetComponent<Renderer>().enabled = visible;
        parenthesisGO2.GetComponent<Renderer>().enabled = visible;
    }

    /// <summary>
    /// 괄호가 있어야 할 경우 true를, 아닐 경우 false를 반환.
    /// 정의하지 않을 경우 기본적으로 항상 false를 반환.
    /// </summary>
    /// <returns>괄호가 보여질 것인지</returns>
    protected virtual bool ShowsParentheses()
    {
        return false;
    }

    protected virtual void SetWidthOfParentheses()
    {
        if (operType == OperType.Operand || operType == OperType.EqualSign)
            return;

        parenthesisGO1.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 2 * widthIntervalBetweenParenthesisAndEdge, height);
        parenthesisGO2.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 2 * widthIntervalBetweenParenthesisAndEdge, height);
    }

    private void SetOrderOfParenthesesAndOperatorTMP()
    {
        if (operType == OperType.Operand)
            return;
        else if (operType == OperType.EqualSign)
        {
            operTMP.GetComponent<Order>()?.SetOrder(order);
            return;
        }

        operTMP.GetComponent<Order>()?.SetOrder(order);
        parenthesisGO1.GetComponent<Order>()?.SetOrder(order);
        parenthesisGO2.GetComponent<Order>()?.SetOrder(order);
    }

    /// <summary>
    /// width와 height를 설정함.
    /// SetSize(w, h)를 이용.
    /// </summary>
    protected abstract void SetSizeOfThis();

    protected void SetSize(float w, float h)
    {
        this.width = w;
        this.height = h;

        this.GetComponent<SpriteRenderer>().size = new Vector2(width, height);
    }

    /// <summary>
    /// 본인의 반투명 정도를 설정 후, 재귀적으로 자식 객체의 함수를 호출(operType == Operand 이면 정지).
    /// </summary>
    /// <param name="isTranlucency"></param>
    public void SetTranslucence(bool isTranlucency)
    {
        if (isTranlucency)
            this.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, degreeOfTranslucence);
        else
            this.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);


        if (operType == OperType.Operand)
            return;

        for (int i = 0; i < childrenOpers.Length; i++)
        {
            childrenOpers[i].SetTranslucence(isTranlucency);
        }
    }

    /// <summary>
    /// 본인의 parentOperator를 설정.
    /// parent == null인 경우, 초기화.
    /// </summary>
    /// <param name="parent">parentOperator</param>
    public void SetParent(OperatorOrOperand parent)
    {
        this.parentOperator = parent;
        if (parent != null)
        {
            this.transform.SetParent(parent.transform, false);
            parent.isUntouched = false;
        }
        else
        {
            this.transform.SetParent(GameObject.Find("CalculationField").transform, true);
        }

    }

    /// <summary>
    /// childNum번째의 childrenOpers 설정.
    /// </summary>
    /// <param name="child"></param>
    /// <param name="childNum"></param>
    public void SetChildren(OperatorOrOperand child, int childNum)
    {
        childrenOpers[childNum] = child;
        childrenOpers[childNum].childNum = childNum;
        emptyOperands[childNum].SetThisVisible(false);
    }

    /// <summary>
    /// childNum번째의 childrenOpers를 Emtpy로 설정. 이후 해당 emptyOperator에서 UpdateOperators() 함수를 호출.
    /// </summary>
    /// <param name="childNum"></param>
    public void SetChildEmpty(int childNum)
    {
        childrenOpers[childNum] = emptyOperands[childNum];
        emptyOperands[childNum].SetThisVisible(true);
        emptyOperands[childNum].UpdateOperators();
    }

    /// <summary>
    /// 자신의 isDragging을 설정한 후, 재귀적으로 자식 객체의 함수를 호출(operType == operand 이면 정지, 자신이 EmptyOperand이면 isDragging 재설정).
    /// DragStart(touchPos)와 DragEnd(touchPos)에서 호출됨.
    /// </summary>
    /// <param name="isDragging"></param>
    protected void CheckIsDragging(bool isDragging)
    {
        this.isDragging = isDragging;

        if (this.operType == OperType.Operand)
            return;

        for (int i = 0; i < childrenOpers.Length; i++)
        {
            childrenOpers[i].CheckIsDragging(isDragging);
        }
    }

    /// <summary>
    /// childrenOpers들과 operTMP, 의 position.x, position.y 값을 정해줌
    /// </summary>
    protected abstract void AlignPosOfExpression();


    /// <summary>
    /// 부모 객체의 order+1 값으로 자신의 order를 설정한 후, 재귀적으로 자식 객체의 함수를 호출(operType == Operand 이면 정지).
    /// Operator 관계에 변경이 있으면 부모 객체에서 최초로 호출됨.
    /// </summary>
    public void SetOrderFromParent()
    {
        this.order = parentOperator != null ? parentOperator.order + 1 : 1;
        this.GetComponent<Order>()?.SetOrder(order);
        SetOrderOfParenthesesAndOperatorTMP();

        if (this.operType == OperType.Operand)
            return;

        for (int i = 0; i < childrenOpers.Length; i++)
        {
            childrenOpers[i].SetOrderFromParent();
        }
    }

    /// <summary>
    /// Order Component의 SetMostFrontOrder(true)를 호출한 후, 재귀적으로 자식 객체의 함수를 호출(operType == Operand 이면 정지).
    /// </summary>
    public void SetMostFrontOrder()
    {
        this.GetComponent<Order>()?.SetMostFrontOrder(true, order);

        if (this.operType == OperType.Operand)
            return;

        for (int i = 0; i < childrenOpers.Length; i++)
        {
            childrenOpers[i].SetMostFrontOrder();
        }
    }

    /// <summary>
    /// 자식 객체에서 width 와 height를 받아와서 조정. 내부의 수식과 괄호도 조정. 재귀적으로 부모 객체의 함수 호출(parentOperator == null 또는 operType == EqualSign이면 정지).
    /// 부모 객체가 바뀌면 부모의 이 함수를 호출함.
    /// </summary>
    public void SetSizeAndPosFromChildren()
    {
        SetSizeOfThis();
        AlignPosOfExpression();
        SetWidthOfParentheses();
        SetParenthesesVisible(ShowsParentheses());

        if (parentOperator == null || operType == OperType.EqualSign)
            return;

        parentOperator.SetSizeAndPosFromChildren();
    }

    /// <summary>
    /// 재귀적으로 자식 객체의 Calculate()로 받아온 인자의 연산값을 반환(operType == Operand이면 정지).
    /// Operator 관계에 변경이 있으면 EqualSign에서 최초로 호출됨.
    /// </summary>
    /// <returns>연산된 정수를 반환. 정수가 아닐 경우 대신 errorNum을 반환하고 CalculateManager의 isError = true로 바꿈.</returns>
    public abstract int Calculate();

    /// <summary>
    /// 변경이 일어난 Operator에서 호출됨.
    /// </summary>
    public void UpdateOperators()
    {
        SetSizeAndPosFromChildren();

        SetOrderFromParent();
    }


    public void MoveTransform(Vector3 pos, bool usesDotween, float dotweenTime = 0)
    {
        if (usesDotween)
        {
            transform.DOMove(pos, dotweenTime);
        }
        else
        {
            transform.position = pos;
        }
    }

    Vector3 GetLocalPositionOfOper()
    {
        return operTMP.transform.localPosition;
    }

    bool ExistsInCalculationArea(Vector3 touchPos)
    {
        touchPos.z = -10;
        RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, Vector3.forward);
        int layer = LayerMask.NameToLayer("CalculationArea");
        return Array.Exists(hits, x => x.collider.gameObject.layer == layer); // hits 중 CalculationArea가 있으면 return true
    }

    /// <summary>
    /// touchPos 위치에서 raycast를 쏴서 DragEnd의 위치를 확인.
    /// emptyOperator의 위치에 이 OperatorOrOperand를 넣음.
    /// </summary>
    /// <param name="touchPos"></param>
    void SetRelationOfParentAndChild(Vector3 touchPos)
    {
        touchPos.z = -10;
        foreach (var hit in Physics2D.RaycastAll(touchPos, Vector3.forward)) // touch한 위치에서 forward로 Raycast를 쏴서 맞은 오브젝트들을 확인
        {
            OperatorOrOperand tmpObject = hit.collider?.GetComponent<OperatorOrOperand>(); // 오브젝트의 collider가 존재하면 OperatorOrOperand를 가져옴
            EmptyOperand emptyOperandObject = tmpObject as EmptyOperand;
            if (emptyOperandObject != null && emptyOperandObject.visible) // 해당 OperatorOrOperand가 EmptyOperator이고, visible 상태인 경우
            {
                if ((emptyOperandObject.parentOperator == parentOperator && emptyOperandObject.childNum == childNum) || emptyOperandObject.isDragging) // 원래 있던 위치인 경우, 또는 드래그 중인 EmptyOperator인 경우(버그)
                {
                    return; // 그대로 유지
                }
                else // 원래 있던 위치가 아닌 경우, 기존의 parentOperator를 초기화하고 새로운 Operator에 끼워넣음
                {
                    parentOperator?.SetChildEmpty(childNum);

                    SetParent(emptyOperandObject.parentOperator);
                    parentOperator.SetChildren(this, emptyOperandObject.childNum);

                    return;
                }
            }
        }

        // 밖으로 뺀 경우
        parentOperator?.SetChildEmpty(childNum);
        SetParent(null);

        touchPos.z = 0;
        MoveTransform(touchPos - GetLocalPositionOfOper(), false);
    }

    /// <summary>
    /// NumberOperand인 경우 SetParent(null)을 함. 재귀적으로 자식 객체의 함수를 호출(operType == Operand이면 정지)
    /// OperatorOrOperand GameObject를 파괴하기 전에 하위의 모든 NumberOperand가 파괴되지 않도록 관리.
    /// </summary>
    public void SuperviseNumberOperandFromBeingDestroyed()
    {
        if(this is NumberOperand)
        {
            SetParent(null);
            DiceManager.instance.MoveDice(this as NumberOperand);
            return;
        }

        if (operType == OperType.Operand)
            return;

        for (int i = 0; i < childrenOpers.Length; i++)
        {
            childrenOpers[i].SuperviseNumberOperandFromBeingDestroyed();
        }
    }

    public void DestroyThisOperator()
    {
        SuperviseNumberOperandFromBeingDestroyed();
        if (operType == OperType.Operator)
            Destroy(this.gameObject);
    }

    public void DragStart(Vector3 touchPos)
    {
        isUntouched = false;
        CheckIsDragging(true);
        SetTranslucence(true);
        SetMostFrontOrder();
    }

    public void Drag(Vector3 touchPos)
    {
        MoveTransform(touchPos - GetLocalPositionOfOper(), false);
        if(this.operType == OperType.Operator)
            CalculateManager.instance.CheckHoverBoxVisible(!ExistsInCalculationArea(touchPos));
    }

    public void DragEnd(Vector3 touchPos)
    {
        CalculateManager.instance.CheckHoverBoxVisible(false);

        SetTranslucence(false);
        SetRelationOfParentAndChild(touchPos);
        UpdateOperators();
        CalculateManager.instance.Calculate();
        CheckIsDragging(false);

        if (!ExistsInCalculationArea(touchPos))
        {
            DestroyThisOperator();
        }
        
    }

    protected virtual void Start()
    {
        SetUpIntialState();
    }
}