using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inven : MonoBehaviour
{
    public Transform inv;
    GameObject limit;
    public GameObject oneslot;
    public GameObject c;
    public int slot = 0;
    bool start = true;
    public static inven instance;
    public GameObject itemPrefab;
    public List<Transform> inventorySlots;
    public GameObject pl;
    public Image bluemushroom;
    public Image copperore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        limit = GameObject.Find("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        slot = limit.GetComponent<player>().limitbag;
        if (start == true && slot != 0)
        {
            creatslot(slot);
            start = false;
        }


    }

    void creatslot(int slot)
    {
        for(int i = 0; i < slot; i++)
        {
           c = Instantiate(oneslot,inv);
           c.name = "slot" + i;
            inventorySlots.Add(c.transform);
        }

    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 아이템을 슬롯에 추가
    public bool AddItem(string itemName)
    {
        foreach (Transform slot in inventorySlots)
        {
            if (slot.childCount == 0)
            {
                GameObject newItem = Instantiate(itemPrefab, slot);
                newItem.transform.localPosition = Vector3.zero;

                return true; 
            }
        }

        Debug.Log("인벤토리 꽉 참");
        return false;
    }
}
