using UnityEngine;

public class chest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform inv;
    public GameObject oneslot;
    public int slot = 0;
    bool start = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        if (start == true && slot != 0)
        {
            creatslot(slot);
            start = false;
        }

    }

    void creatslot(int slot)
    {
        for (int i = 0; i < slot; i++)
        {

            GameObject c = Instantiate(oneslot, inv);
            c.name = "slot" + i;
        }

    }
}
