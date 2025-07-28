using UnityEngine;

public class invenoper : MonoBehaviour
{
    public GameObject inventory;
    bool oper = false;
    float time = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && oper == false && time < 0)
        {
            time = 0.5f;
            inventory.SetActive(true);
            oper = true;
        }


        if (Input.GetKeyDown(KeyCode.E) && oper == true && time < 0)
        {
            time = 0.5f;
            inventory.SetActive(false);
            oper = false;
        }
    }
}
