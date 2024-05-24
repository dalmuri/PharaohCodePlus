using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public enum OperType { Operand, Operator, EqualSign } // +-����^��!
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
    [SerializeField] public int childNum = 0; // ������ �θ� ��ü�� �� ��° child����
    [SerializeField] public OperatorOrOperand[] childrenOpers;

    [SerializeField] public int order = 1; // sortingOrder

    public bool isUntouched { get; set; } = true;
    public bool canDrag { get; set; } = true;
    public bool isDragging = false;

    /// <summary>
    /// ������ ���¸� ����. Start()���� ȣ��.
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
    /// ��ȣ�� �־�� �� ��� true��, �ƴ� ��� false�� ��ȯ.
    /// �������� ���� ��� �⺻������ �׻� false�� ��ȯ.
    /// </summary>
    /// <returns>��ȣ�� ������ ������</returns>
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
    /// width�� height�� ������.
    /// SetSize(w, h)�� �̿�.
    /// </summary>
    protected abstract void SetSizeOfThis();

    protected void SetSize(float w, float h)
    {
        this.width = w;
        this.height = h;

        this.GetComponent<SpriteRenderer>().size = new Vector2(width, height);
    }

    /// <summary>
    /// ������ ������ ������ ���� ��, ��������� �ڽ� ��ü�� �Լ��� ȣ��(operType == Operand �̸� ����).
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
    /// ������ parentOperator�� ����.
    /// parent == null�� ���, �ʱ�ȭ.
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
    /// childNum��°�� childrenOpers ����.
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
    /// childNum��°�� childrenOpers�� Emtpy�� ����. ���� �ش� emptyOperator���� UpdateOperators() �Լ��� ȣ��.
    /// </summary>
    /// <param name="childNum"></param>
    public void SetChildEmpty(int childNum)
    {
        childrenOpers[childNum] = emptyOperands[childNum];
        emptyOperands[childNum].SetThisVisible(true);
        emptyOperands[childNum].UpdateOperators();
    }

    /// <summary>
    /// �ڽ��� isDragging�� ������ ��, ��������� �ڽ� ��ü�� �Լ��� ȣ��(operType == operand �̸� ����, �ڽ��� EmptyOperand�̸� isDragging �缳��).
    /// DragStart(touchPos)�� DragEnd(touchPos)���� ȣ���.
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
    /// childrenOpers��� operTMP, �� position.x, position.y ���� ������
    /// </summary>
    protected abstract void AlignPosOfExpression();


    /// <summary>
    /// �θ� ��ü�� order+1 ������ �ڽ��� order�� ������ ��, ��������� �ڽ� ��ü�� �Լ��� ȣ��(operType == Operand �̸� ����).
    /// Operator ���迡 ������ ������ �θ� ��ü���� ���ʷ� ȣ���.
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
    /// Order Component�� SetMostFrontOrder(true)�� ȣ���� ��, ��������� �ڽ� ��ü�� �Լ��� ȣ��(operType == Operand �̸� ����).
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
    /// �ڽ� ��ü���� width �� height�� �޾ƿͼ� ����. ������ ���İ� ��ȣ�� ����. ��������� �θ� ��ü�� �Լ� ȣ��(parentOperator == null �Ǵ� operType == EqualSign�̸� ����).
    /// �θ� ��ü�� �ٲ�� �θ��� �� �Լ��� ȣ����.
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
    /// ��������� �ڽ� ��ü�� Calculate()�� �޾ƿ� ������ ���갪�� ��ȯ(operType == Operand�̸� ����).
    /// Operator ���迡 ������ ������ EqualSign���� ���ʷ� ȣ���.
    /// </summary>
    /// <returns>����� ������ ��ȯ. ������ �ƴ� ��� ��� errorNum�� ��ȯ�ϰ� CalculateManager�� isError = true�� �ٲ�.</returns>
    public abstract int Calculate();

    /// <summary>
    /// ������ �Ͼ Operator���� ȣ���.
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
        return Array.Exists(hits, x => x.collider.gameObject.layer == layer); // hits �� CalculationArea�� ������ return true
    }

    /// <summary>
    /// touchPos ��ġ���� raycast�� ���� DragEnd�� ��ġ�� Ȯ��.
    /// emptyOperator�� ��ġ�� �� OperatorOrOperand�� ����.
    /// </summary>
    /// <param name="touchPos"></param>
    void SetRelationOfParentAndChild(Vector3 touchPos)
    {
        touchPos.z = -10;
        foreach (var hit in Physics2D.RaycastAll(touchPos, Vector3.forward)) // touch�� ��ġ���� forward�� Raycast�� ���� ���� ������Ʈ���� Ȯ��
        {
            OperatorOrOperand tmpObject = hit.collider?.GetComponent<OperatorOrOperand>(); // ������Ʈ�� collider�� �����ϸ� OperatorOrOperand�� ������
            EmptyOperand emptyOperandObject = tmpObject as EmptyOperand;
            if (emptyOperandObject != null && emptyOperandObject.visible) // �ش� OperatorOrOperand�� EmptyOperator�̰�, visible ������ ���
            {
                if ((emptyOperandObject.parentOperator == parentOperator && emptyOperandObject.childNum == childNum) || emptyOperandObject.isDragging) // ���� �ִ� ��ġ�� ���, �Ǵ� �巡�� ���� EmptyOperator�� ���(����)
                {
                    return; // �״�� ����
                }
                else // ���� �ִ� ��ġ�� �ƴ� ���, ������ parentOperator�� �ʱ�ȭ�ϰ� ���ο� Operator�� ��������
                {
                    parentOperator?.SetChildEmpty(childNum);

                    SetParent(emptyOperandObject.parentOperator);
                    parentOperator.SetChildren(this, emptyOperandObject.childNum);

                    return;
                }
            }
        }

        // ������ �� ���
        parentOperator?.SetChildEmpty(childNum);
        SetParent(null);

        touchPos.z = 0;
        MoveTransform(touchPos - GetLocalPositionOfOper(), false);
    }

    /// <summary>
    /// NumberOperand�� ��� SetParent(null)�� ��. ��������� �ڽ� ��ü�� �Լ��� ȣ��(operType == Operand�̸� ����)
    /// OperatorOrOperand GameObject�� �ı��ϱ� ���� ������ ��� NumberOperand�� �ı����� �ʵ��� ����.
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