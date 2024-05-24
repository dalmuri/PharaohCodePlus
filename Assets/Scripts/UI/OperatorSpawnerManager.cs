using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorSpawnerManager : MonoBehaviour
{
    public static OperatorSpawnerManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }


    [SerializeField] GameObject operatorSpawnerBtnPrefab;
    [SerializeField] GameObject operatorSpawnerScrollGO;
    [SerializeField] GameObject canvas;


    void AddOperatorSpawnerBtn(OperatorType operatorType)
    {
        GameObject operatorSpawnerBtnObject = Instantiate(operatorSpawnerBtnPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        operatorSpawnerBtnObject.transform.SetParent(operatorSpawnerScrollGO.transform, false);
        OperatorSpawnerBtn operatorSpawnerBtn = operatorSpawnerBtnObject.GetComponent<OperatorSpawnerBtn>();
        operatorSpawnerBtn.SetUp(operatorType);
    }

    void SetUpOperatorSpawner()
    {
        foreach(OperatorType operatorType in System.Enum.GetValues(typeof(OperatorType)))
        {
            AddOperatorSpawnerBtn(operatorType);
        }
    }

    void Start()
    {
        SetUpOperatorSpawner();
    }
}
