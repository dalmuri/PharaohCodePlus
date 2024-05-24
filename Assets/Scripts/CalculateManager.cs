using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public enum OperatorType
{
    Addition = 0, Subtraction, Multiplication, Division, Exponentiation, Root, Factorial
}

public enum ErrorType
{
    /// <summary>EmptyOperand�� �����ϴ� ���</summary>
    Empty = 0,
    /// <summary>0���� ���� ���</summary>
    DivideZero,
    /// <summary>Root�� ����� ������ �ƴ� ���</summary>
    RootNotInt,
    /// <summary>�������� ����� ������ �ƴ� ���</summary>
    DivisionNotInt,
    /// <summary>������ Root�� ���</summary>
    NegativeRoot,
    /// <summary>������ ������ ���</summary>
    NegativeExponent,
    /// <summary>������ ���� ��� 0�� ���</summary>
    ZeroToThePowerOfZero,
    /// <summary>������ Factorial�� ���</summary>
    NegativeFactorial,
    /// <summary>Factorial�� ����� Integer ������ �Ѵ� ���(12! < 2,147,483,647 < 13!)</summary>
    FactorialOutOfRange,
    /// <summary>���� ��� ����� 999 �ʰ� �Ǵ� -999 �̸��� ���</summary>
    ResultOver999,
    /// <summary>1�� ������ ���ڸ� �Ἥ ����� ���� ���</summary>
    UsedUnder2Numbers
}

public class CalculateManager : MonoBehaviour
{
    public static CalculateManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }


    [SerializeField] EqualSign equalSign;

    public bool[] existsError;
    public int numberOfUsedNumberOperand = 0;
    [SerializeField] GameObject calculationFieldGO;
    [SerializeField] Transform calculationOperatorSpawnTransform;
    [SerializeField] TMP_Text resultTMP;
    [SerializeField] Renderer hoverBox;
    [SerializeField] TextMeshProUGUI errorTMP;

    [SerializeField] GameObject[] OperatorPrefabs;

    [SerializeField] GameObject operatorSpawnerBtnPrefab;
    [SerializeField] GameObject operatorSpawnerScrollGO;
    [SerializeField] GameObject canvas;

    OperatorOrOperand latestSpawnedOperator;
    int errorTypeCount;
    public int result { get; private set; } = 0;
    public int NumberOfCorrectCalculation = 0;

    const float errorFadeAwayTime = 2f;

    void SetUpInitialState()
    {
        errorTMP.alpha = 0f;
        errorTypeCount = System.Enum.GetValues(typeof(ErrorType)).Length;
        existsError = new bool[errorTypeCount];
        SetResultVisible(false);
        DiceManager.RollDiceEvent += DestroyAllOperInEqualSign;

        foreach (OperatorType operatorType in System.Enum.GetValues(typeof(OperatorType)))
        {
            AddOperatorSpawnerBtn(operatorType);
        }
    }

    public void CheckHoverBoxVisible(bool visible)
    {
        if (visible)
            hoverBox.enabled = true;
        else
            hoverBox.enabled = false;
    }

    void SetResultVisible(bool visible)
    {
        resultTMP.GetComponent<Renderer>().enabled = visible;
    }

    public void DestroyAllOperInEqualSign()
    {
        if(equalSign.childrenOpers[0] is not EmptyOperand)
        {
            equalSign.childrenOpers[0].DestroyThisOperator();
            equalSign.SetChildEmpty(0);
        }
        SetResultVisible(false);
    }

    public void Calculate()
    {
        for(int i=0; i<errorTypeCount; i++)
            existsError[i] = false;
        numberOfUsedNumberOperand = 0;

        result = equalSign.Calculate();

        for(int i=0; i<errorTypeCount; i++)
        {
            if (existsError[i])
            {
                ShowCalculationErrorMessage();
                result = 0;
                SetResultVisible(false);
                return;
            }
        }

        resultTMP.text = result.ToString();
        SetResultVisible(true);
        NumberOfCorrectCalculation += 1;
    }

    public void ShowCalculationErrorMessage()
    {
        ErrorType errorType = ErrorType.Empty;
        for (int i = 0; i < errorTypeCount; i++)
        {
            if (existsError[i])
            {
                errorType = (ErrorType)i;
                break;
            }
        }

        errorTMP.text = errorType switch
        {
            ErrorType.Empty => "",
            ErrorType.DivideZero => "",
            ErrorType.RootNotInt => "",
            ErrorType.DivisionNotInt => "",
            ErrorType.NegativeRoot => "",
            ErrorType.NegativeExponent => "",
            ErrorType.ZeroToThePowerOfZero => "",
            ErrorType.NegativeFactorial => "",
            ErrorType.FactorialOutOfRange => "",
            ErrorType.ResultOver999 => "������� �� �ڸ� ���� ���� �ʰ� ���ּ���.",
            ErrorType.UsedUnder2Numbers => "2�� �̻��� ���ڸ� ������ּ���.",
            _ => "",
        };

        errorTMP.DOKill(false);
        errorTMP.alpha = 1f;
        errorTMP.DOFade(0f, errorFadeAwayTime).SetEase(Ease.InExpo);
    }
    #region OperatorSpawn

    public void SpawnCalculationOperator(OperatorType operatorType)
    {
        if (latestSpawnedOperator != null && latestSpawnedOperator.isUntouched)
        {
            Destroy(latestSpawnedOperator.gameObject);
        }

        GameObject operatorObject = Instantiate(OperatorPrefabs[(int)operatorType], Vector3.zero, Quaternion.identity);
        operatorObject.transform.SetParent(calculationFieldGO.transform, false);
        operatorObject.transform.position = calculationOperatorSpawnTransform.position;

        latestSpawnedOperator = operatorObject.GetComponent<OperatorOrOperand>();
    }

    void AddOperatorSpawnerBtn(OperatorType operatorType)
    {
        GameObject operatorSpawnerBtnObject = Instantiate(operatorSpawnerBtnPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        operatorSpawnerBtnObject.transform.SetParent(operatorSpawnerScrollGO.transform, false);
        OperatorSpawnerBtn operatorSpawnerBtn = operatorSpawnerBtnObject.GetComponent<OperatorSpawnerBtn>();
        operatorSpawnerBtn.SetUp(operatorType);
    }
    #endregion

    private void Start()
    {
        SetUpInitialState();
    }

    private void OnDestroy()
    {
        DiceManager.RollDiceEvent -= DestroyAllOperInEqualSign;
    }
}
