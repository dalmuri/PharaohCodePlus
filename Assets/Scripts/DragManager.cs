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
        // ��ġ ����
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)) && !isDragging && canAllDrag)
        {
            Vector3 touchPos = Input.GetMouseButtonDown(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            
            foreach (var hit in Physics2D.RaycastAll(touchPos, Vector3.forward)) // touch�� ��ġ���� forward�� Raycast�� ���� ���� ������Ʈ���� Ȯ��
            {
                IDraggableObject tmpObject = hit.collider?.GetComponent<IDraggableObject>(); // ������Ʈ�� collider�� �����ϸ� DraggableObject�� ������
                if (tmpObject != null && tmpObject?.canDrag == true) // �ش� DraggableObject�� �����ϸ�
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

        // ��ġ ��
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

        // ��ġ ��
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