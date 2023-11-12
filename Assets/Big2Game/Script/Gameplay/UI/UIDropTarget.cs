using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

public class UIDropTarget : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        // Handle the drop event here
        Debug.Log("Dropped an object");
        // You can invoke a UniRx event or perform any other action.
        // For example, using UniRx's Subject:
        //nextAction.OnNext("Object Dropped");
    }
}