using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class invenitem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    public Transform originalParent;
    private CanvasGroup canvasGroup;
    public int down;
    public int up;
    public int right;
    public int left;
    public int square;
    public int esquare;
    bool reoper = false;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        this.gameObject.tag = "item";
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalParent);
        }

        GameObject target = eventData.pointerEnter;

        if (target != null && target.gameObject.tag == "slot")
        {
            Debug.Log("ΩΩ∑‘ ¿÷¿Ω");
            if(reoper == false)
            {
                transform.SetParent(target.transform);
                originalParent = target.transform;
                this.gameObject.tag = "Untagged";
                reoper = false;
            }
        }
        else
        {
            Debug.Log("ΩΩ∑‘ æ¯¿Ω");
            transform.SetParent(originalParent);
            Vector3 ret = new Vector3(originalParent.transform.position.x, originalParent.transform.position.y, originalParent.transform.position.z);
            transform.position = ret;
            this.gameObject.tag = "Untagged";
        }

    }

    public void itemreturn()
    {
        transform.SetParent(originalParent);
        Vector3 ret = new Vector3(originalParent.transform.position.x, originalParent.transform.position.y, originalParent.transform.position.z);
        transform.position = ret;
        reoper = true;
    }
}
