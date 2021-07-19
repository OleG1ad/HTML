using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomSlider : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform fillRect;

    //public SiteProgressLoading siteProgressLoading;    

    private RectTransform canvasRect, rectTransform;

    // по ширине координаты X 
    private float fillWidth, imageWidth, minPosX, maxPosX, targetPosX;

    private bool isDragging, isClose;

    public void Close()
    {
        if (!isClose && !isDragging) isClose = true;


    }

    public void Open()
    {
        if (isClose && !isDragging) isClose = false;
    }

    protected override void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.transform as RectTransform;

        rectTransform = transform as RectTransform;        

        imageWidth = rectTransform.sizeDelta.x;

        // слева на право от угла 1 (верх слева) до угла 2 (верх справа)
        Vector3[] fillCorners = new Vector3[4];
        fillRect.GetLocalCorners(fillCorners);
        fillWidth = Mathf.Abs(fillCorners[1].x * 2f);
        maxPosX = fillCorners[1].x + imageWidth / 2f;
        minPosX = fillCorners[2].x - imageWidth / 2f;
    }

    private void Update()
    {        
            UpdateFill();
            UpdatePosition();        
    }

    private void UpdateFill()
    {
        float value = GetFillValue();
        float newSizeX = fillWidth * value;

        RectTransform.Edge edge = RectTransform.Edge.Left;
        fillRect.SetInsetAndSizeFromParentEdge(edge, 0f, newSizeX);
    }

    private float GetFillValue()
    {
        float currentXPos = -(rectTransform.localPosition.x);
        float diff = currentXPos - minPosX;
        float result = -(diff / (fillWidth - imageWidth));
        return result;
    }

    private void UpdatePosition()
    {
        Vector2 currentPos = rectTransform.localPosition;

        if (!isDragging)
        {
            float newXPos = (isClose) ? maxPosX : minPosX;
            float speed = Time.deltaTime * /*10f*/5f;
            targetPosX = Mathf.Lerp(currentPos.x, newXPos, speed);
        }

        float xPos = Mathf.Clamp(targetPosX, maxPosX, minPosX);
        rectTransform.localPosition = new Vector2(xPos, currentPos.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera eventCam = eventData.pressEventCamera;
        Vector2 worldPoint = eventCam.ScreenToWorldPoint(eventData.position);
        Vector2 localPoint = canvasRect.InverseTransformPoint(worldPoint);
        targetPosX = localPoint.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
