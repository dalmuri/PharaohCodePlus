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
    /// <summary>EmptyOperand가 존재하는 경우</summary>
    Empty = 0,
    /// <summary>0으로 나눈 경우</summary>
    DivideZero,
    /// <summary>Root의 결과가 정수가 아닌 경우</summary>
    RootNotInt,
    /// <summary>나눗셈의 결과가 정수가 아닌 경우</summary>
    DivisionNotInt,
    /// <summary>음수의 Root인 경우</summary>
    NegativeRoot,
    /// <summary>지수가 음수인 경우</summary>
    NegativeExponent,
    /// <summary>지수와 밑이 모두 0인 경우</summary>
    ZeroToThePowerOfZero,
    /// <summary>음수의 Factorial인 경우</summary>
    NegativeFactorial,
    /// <summary>Factorial의 결과가 Integer 범위를 넘는 경우(12! < 2,147,483,647 < 13!)</summary>
    FactorialOutOfRange,
    /// <summary>최종 계산 결과가 999 초과 또는 -999 미만인 경우</summary>
    ResultOver999,
    /// <summary>1개 이하의 숫자만 써서 결과를 만든 경우</summary>
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
            ErrorType.ResultOver999 => "결과값이 네 자리 수를 넘지 않게 해주세요.",
            ErrorType.UsedUnder2Numbers => "2개 이상의 숫자를 사용해주세요.",
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
