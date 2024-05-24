using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSquareManager : MonoBehaviour
{
    public static NumberSquareManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    Vector3 pyramidPos;
    const float intervalX = 1.3f;
    const float intervalY = 1.25f;
    const float intervalBetweenFloor0AndPyramidY = 2.3f;
    readonly int[] floorMaxCount = { 4, 3, 2, 1 };
    const float delayDraw = 0.15f;

    [SerializeField] NumberInfoSO numberInfoSO;
    [SerializeField] GameObject numberSquarePrefab;
    [SerializeField] GameObject pyramidGO;
    [SerializeField] NumberSquare emptyNumberSquareObject;

    List<NumberInfo>[] deckNSs; // 0층, 1층, 2층, 3층
    NumberSquare[][] pyramidNSs;

    public bool isPyramidFull { get; private set; } = false;

    #region Pyramid

    private void SetUpInitialPyramid()
    {
        pyramidPos = pyramidGO.transform.position;

        pyramidNSs = new NumberSquare[4][] { new NumberSquare[4] { emptyNumberSquareObject, emptyNumberSquareObject, emptyNumberSquareObject, emptyNumberSquareObject },
                                             new NumberSquare[3] { emptyNumberSquareObject, emptyNumberSquareObject, emptyNumberSquareObject },
                                             new NumberSquare[2] { emptyNumberSquareObject, emptyNumberSquareObject },
                                             new NumberSquare[1] { emptyNumberSquareObject }
                                           };
        for (int floor = 0; floor < 4; floor++)
            DeckDisplayManager.instance.SetDisplayNumber(floor, deckNSs[floor].Count);
    }

    public IEnumerator FillPyramidCo() // pyramid에 부족한 만큼 Number 채우기. 모두 채웠으면 isPyramidFull = true, 하나라도 못 채웠으면 isPyramidFull = false
    {
        bool tempIsPyramidFull = true;

        for (int floor = 0; floor < 4; floor++)
        {
            while (CountFloor(floor) < floorMaxCount[floor] && deckNSs[floor].Count > 0) // floor층의 NumberSquare가 부족하고, floor층을 채울 덱에 Number가 있는 경우
            {
                Vector3 spawnPos = DeckDisplayManager.instance.GetPosOfDeckDisplay(floor);

                // deckNSs의 맨 위 하나를 뽑아 NumberSquare를 생성함
                GameObject numberSquareObject = Instantiate(numberSquarePrefab, Vector3.zero, Quaternion.identity);
                numberSquareObject.transform.SetParent(pyramidGO.transform, false);
                numberSquareObject.transform.position = spawnPos;
                NumberSquare numberSquare = numberSquareObject.GetComponent<NumberSquare>();
                numberSquare.SetUp(deckNSs[floor][0], false);
                deckNSs[floor].RemoveAt(0);

                // 해당 floor의 비어 있는 첫 위치에 NumberSquare를 이동시켜줌
                for (int i = 0; i < floorMaxCount[floor]; i++)
                {
                    if (pyramidNSs[floor][i] == emptyNumberSquareObject /*|| pyramidNSs[floor][i].squareColor == SquareColor.Empty*/)
                    {
                        pyramidNSs[floor][i] = numberSquare;
                        numberSquare.indexOfFloor = i;
                        numberSquare.originPos = GetPosOfNumberSquare(floor, i);
                        numberSquare.MoveTransform(numberSquare.originPos, true, delayDraw);
                        DeckDisplayManager.instance.SetDisplayNumber(floor, deckNSs[floor].Count);
                        yield return new WaitForSeconds(delayDraw);
                        break;
                    }
                }
            }

            if (CountFloor(floor) < floorMaxCount[floor]) // floor층이 완전히 채워지지 않은 경우
                tempIsPyramidFull = false;
        }

        isPyramidFull = tempIsPyramidFull;
    }

    private int CountFloor(int floor)
    {        
        int count = 0;
        for (int i = 0; i < pyramidNSs[floor].Length; i++)
        {
            if (pyramidNSs[floor][i] != emptyNumberSquareObject)
                count++;

        }
        
        return count;
    }

    private Vector3 GetPosOfNumberSquare(int floor, int index)
    {
        float posX = pyramidPos.x + (floorMaxCount[floor] - 1) * (-intervalX/2f) + index * intervalX;
        float posY = pyramidPos.y - intervalBetweenFloor0AndPyramidY + floor * intervalY;

        return new Vector3(posX, posY, 0);
    }

    public void EmptyOutNumberSquare(int floor, int index)
    {
        if (floor < 0 || floor >= 4 || index < 0 || index >= floorMaxCount[floor])
            return;

        Destroy(pyramidNSs[floor][index].gameObject);
        pyramidNSs[floor][index] = emptyNumberSquareObject;
    }

    public void SetNumberSquareDraggable(bool draggable)
    {
        for (int floor = 0; floor < 4; floor++)
        {
            for (int i = 0; i < floorMaxCount[floor]; i++)
            {
                pyramidNSs[floor][i].canDrag = draggable;
            }
        }
    }
    #endregion

    private void SetUpInitialDeck()
    {
        deckNSs = new List<NumberInfo>[4];
        for (int i = 0; i < deckNSs.Length; i++)
            deckNSs[i] = new List<NumberInfo>();

        for (int i = 0; i < numberInfoSO.numberInfos.Length; i++)
        {
            NumberInfo numberInfo = numberInfoSO.numberInfos[i];
            switch (numberInfo.squareColor)
            {
                case SquareColor.Yellow:
                    deckNSs[0].Add(numberInfo);
                    break;
                case SquareColor.Blue:
                    deckNSs[1].Add(numberInfo);
                    break;
                case SquareColor.Red:
                    deckNSs[2].Add(numberInfo);
                    break;
                case SquareColor.Purple:
                    deckNSs[3].Add(numberInfo);
                    break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            ShuffleList(deckNSs[i]);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        int last = list.Count - 2;
        for (int i = 0; i < last; i++)
        {
            int rand = Random.Range(i, list.Count);
            Swap(i, rand);
        }

        void Swap(int a, int b) // local method Swap
        {
            T temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }

    void Start()
    {
        SetUpInitialDeck();
        SetUpInitialPyramid();
    }
}