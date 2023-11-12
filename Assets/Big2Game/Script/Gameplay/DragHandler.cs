using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	Transform startParent;

	public void OnEndDrag(PointerEventData eventData)
	{
		itemBeingDragged = null;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		if (transform.parent == startParent)
		{
			transform.position = startPosition;
			//		  Debug.Log("not Parent..............");
		}
		else
		{
			//			Debug.Log("........ Parent..............transform.parent : " + transform.parent.gameObject.GetInstanceID() + " : " + startParent.gameObject.GetInstanceID());
		}

	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;

		//		Debug.Log("........ OnBeginDrag.............. : " + startParent.gameObject.GetInstanceID());

		GetComponent<CanvasGroup>().blocksRaycasts = false;

	}

}