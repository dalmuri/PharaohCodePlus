using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragManager : MonoBehaviour
{
    public static DragManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    IDraggableObject selectObject;
    public bool canAllDrag;
    public bool isDragging;

    void Start()
    {
        canAllDrag = true;
        isDragging = false;
    }

    public static event Action<Vector3, IDraggableObject> Dragging;

    void Update()
    {
        // 터치 시작
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)) && !isDragging && canAllDrag)
        {
            Vector3 touchPos = Input.GetMouseButtonDown(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            
            foreach (var hit in Physics2D.RaycastAll(touchPos, Vector3.forward)) // touch한 위치에서 forward로 Raycast를 쏴서 맞은 오브젝트들을 확인
            {
                IDraggableObject tmpObject = hit.collider?.GetComponent<IDraggableObject>(); // 오브젝트의 collider가 존재하면 DraggableObject를 가져옴
                if (tmpObject != null && tmpObject?.canDrag == true) // 해당 DraggableObject가 존재하면
                {
                    isDragging = true;
                    selectObject = tmpObject;
                    break;
                }
            }

            if (selectObject?.canDrag == true)
            {
                touchPos.z = 0;
                selectObject?.DragStart(touchPos);
                isDragging = true;
            }
        }

        // 터치 중
        if((Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)) && isDragging && canAllDrag)
        {
            if(selectObject != null)
            {
                Vector3 touchPos = Input.GetMouseButton(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                touchPos.z = 0;

                selectObject?.Drag(touchPos);

                Dragging?.Invoke(touchPos, selectObject);
            }
        }

        // 터치 끝
        if((Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isDragging)
        {
            Vector3 touchPos = Input.GetMouseButtonUp(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPos.z = 0;

            selectObject?.DragEnd(touchPos);
            isDragging = false;
            selectObject = null;
        }
    }
}

public interface IDraggableObject
{
    public bool canDrag { get; set; }
    public void DragStart(Vector3 touchPos);

    public void Drag(Vector3 touchPos);

    public void DragEnd(Vector3 touchPos);
}