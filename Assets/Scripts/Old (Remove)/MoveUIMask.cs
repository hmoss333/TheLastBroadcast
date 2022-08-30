using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveUIMask : MonoBehaviour
{
    private bool mouseDown = false;
    private Vector3 startMousePos;
    private Vector3 startPos;
    private bool restrictX;
    private bool restrictY;
    private float fakeX;
    private float fakeY;
    private float myWidth;
    private float myHeight;
    private bool snapHome = true;

    public RectTransform ParentRT;
    public RectTransform MyRect;
    public RectTransform MyChild;
    private Vector3 ChildPos;

    void Start()
    {
        myWidth = (MyRect.rect.width + 5) / 2;
        myHeight = (MyRect.rect.height + 5) / 2;
        ChildPos = MyChild.transform.position;
    }


    public void OnPointerDown(PointerEventData ped)
    {
        mouseDown = true;
        startPos = transform.position;
        startMousePos = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData ped)
    {
        mouseDown = false;

        if (snapHome)
            transform.position = startPos;
    }


    void Update()
    {
        if (mouseDown)
        {
            Vector3 currentPos = Input.mousePosition;
            Vector3 diff = currentPos - startMousePos;
            Vector3 pos = startPos + diff;
            transform.position = pos;
            MyChild.transform.position = ChildPos;

            if (transform.localPosition.x < 0 - ((ParentRT.rect.width / 2) - myWidth) || transform.localPosition.x > ((ParentRT.rect.width / 2) - myWidth))
                restrictX = true;
            else
                restrictX = false;

            if (transform.localPosition.y < 0 - ((ParentRT.rect.height / 2) - myHeight) || transform.localPosition.y > ((ParentRT.rect.height / 2) - myHeight))
                restrictY = true;
            else
                restrictY = false;

            if (restrictX)
            {
                if (transform.localPosition.x < 0)
                    fakeX = 0 - (ParentRT.rect.width / 2) + myWidth;
                else
                    fakeX = (ParentRT.rect.width / 2) - myWidth;

                Vector3 xpos = new Vector3(fakeX, transform.localPosition.y, 0.0f);
                transform.localPosition = xpos;
            }

            if (restrictY)
            {
                if (transform.localPosition.y < 0)
                    fakeY = 0 - (ParentRT.rect.height / 2) + myHeight;
                else
                    fakeY = (ParentRT.rect.height / 2) - myHeight;

                Vector3 ypos = new Vector3(transform.localPosition.x, fakeY, 0.0f);
                transform.localPosition = ypos;
            }

        }
    }
}
