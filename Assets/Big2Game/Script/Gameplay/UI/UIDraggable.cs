using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image thisImage;
    Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        thisImage.raycastTarget = false;
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        thisImage.raycastTarget = false;
        transform.position = startPosition;
    }
}