using UnityEngine;
using UnityEngine.EventSystems;

public class eqipmentslot : MonoBehaviour, IDropHandler
{

    public bool eqip = false;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped != null)
        {
            dropped.transform.SetParent(transform);
            dropped.transform.localPosition = Vector3.zero;
            eqip = true;

        }
    }
}
