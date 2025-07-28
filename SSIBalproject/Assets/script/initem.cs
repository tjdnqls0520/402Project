using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class initem : MonoBehaviour, IDropHandler
{
    int r;
    int l;
    int u;
    int d;
    int slotnumber = 0;
    public int child = 0;
    public int relr;
    public int reud;
    public bool exinven = false;
    bool itemu = false;
    GameObject slotfinder;
    GameObject itemfinder;
    string slot;
    public int slotIndex = -1;
    int slotchild;

    void Start()
    {
        slotIndex = ExtractSlotNumber(gameObject.name);
        Debug.Log(slotIndex);
    }

    int ExtractSlotNumber(string name)
    {
        Match match = Regex.Match(name, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            Debug.LogWarning(name);
            return -1;
        }
    }
    void Update()
    {
       
        if (transform.childCount == child)
        {
            this.gameObject.tag = "slot";
        }
        else
        {
            this.gameObject.tag = "Untagged";
        }

        itemfinder = GameObject.FindWithTag("item");
    }

    
   

    public void OnDrop(PointerEventData eventData)
    {

        GameObject dropped = eventData.pointerDrag;

        if (dropped != null)
        {
            dropped.transform.SetParent(transform); 
            dropped.transform.localPosition = Vector3.zero;

            //정사각형 인벤토리 공간 차지
            if (dropped.GetComponent<invenitem>().square > 0)
            {
                slotnumber = 0;
                for (d = dropped.GetComponent<invenitem>().square; d > 0; d--)
                {
                    slotnumber += 9;
                    slotchild = slotIndex + slotnumber;
                    slot = "slot" + slotchild;
                    slotfinder = GameObject.Find(slot);
                    Debug.Log(slotfinder);
                    if (slotfinder != null && slotfinder.gameObject.tag != "Untagged" && itemu == false)
                    {
                        Debug.Log("작동함");
                        slotfinder.GetComponent<initem>().child = 1;
                    }
                    else
                    {
                        itemu = true;
                        itemfinder.GetComponent<invenitem>().itemreturn();
                    }
                }

               
                slotnumber = 0;
                for(int i = dropped.GetComponent<invenitem>().square + 1; i > 0; i--)
                {
                    
                    for (r = dropped.GetComponent<invenitem>().square; r > 0; r--)
                    {
                        slotnumber += 1;
                        slotchild = slotIndex + slotnumber;
                        slot = "slot" + slotchild;
                        slotfinder = GameObject.Find(slot);
                        Debug.Log(slotfinder);
                        if ( slotfinder != null && slotfinder.gameObject.tag != "Untagged" && itemu == false)
                        {
                            slotfinder.GetComponent<initem>().child = 1;
                        }
                        else
                        {
                            itemu = true;
                            itemfinder.GetComponent<invenitem>().itemreturn();
                        }
                    }
                    slotnumber += 9 - dropped.GetComponent<invenitem>().square;
                }
               
            }

            //직사각형 인벤토리 차지
            if (dropped.GetComponent<invenitem>().esquare > 0)
            {
                slotnumber = 0;
                for (d = dropped.GetComponent<invenitem>().down + 1; d > 0; d--)
                {
                    slotchild = slotIndex + slotnumber;
                    slot = "slot" + slotchild;
                    slotfinder = GameObject.Find(slot);
                    Debug.Log(slotfinder);
                    if (slotfinder != null && slotfinder.gameObject.tag != "Untagged" && itemu == false)
                    {
                        slotfinder.GetComponent<initem>().child = 1;
                    }
                    else
                    {
                        itemu = true;
                        itemfinder.GetComponent<invenitem>().itemreturn();
                    }

                    for (r = dropped.GetComponent<invenitem>().right; r > 0; r--)
                    {
                        slotnumber += 1;
                        slotchild = slotIndex + slotnumber;
                        slot = "slot" + slotchild;
                        slotfinder = GameObject.Find(slot);
                        Debug.Log(slotfinder);
                        if (slotfinder != null && slotfinder.gameObject.tag != "Untagged" && itemu == false)
                        {
                            slotfinder.GetComponent<initem>().child = 1;
                        }
                        else
                        {
                            itemu = true;
                            itemfinder.GetComponent<invenitem>().itemreturn();
                        }
                    }

                    slotnumber += 9 - dropped.GetComponent<invenitem>().right;
                }
            }
        }
    }
}
