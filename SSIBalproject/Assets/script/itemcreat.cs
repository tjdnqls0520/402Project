using UnityEngine;

public class itemcreat : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform itemParent;
    public int itemCount = 5;

    void Start()
    {
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(itemPrefab, itemParent);
            item.name = "Item_" + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
