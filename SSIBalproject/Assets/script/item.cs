using System.Collections.Generic;
using System.IO;
using UnityEngine;



[System.Serializable]
public class idata 
{
    public string type;
    public string ID;
    public string name;
    public float time;
    public float weight;
    public float bag;
    public float need_stm;
    public float chest_drop;
    public float need_more;
    public float muukum;
}

[System.Serializable]
public class idataListWrapper
{
    public List<idata> list;
}


public class item : MonoBehaviour
{
    public string id = "g_01_01_002";
    public string itable;
    public string Name;
    public float time;
    public float weight;
    public float bag;
    public float needstm;
    public float chestdrop;
    public float needmore;
    public float muukum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itable = Path.Combine(Application.streamingAssetsPath, "json/itype.json");

        idata selected = loadit(id);
        Name = selected.name;
        time = selected.time;
        weight = selected.weight;
        bag = selected.bag;
        needstm = selected.need_stm;
        chestdrop = selected.chest_drop;
        needmore = selected.need_more;
        muukum = selected.muukum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    idata loadit(string id)
    {
        try
        {
            if (File.Exists(itable))
            {
                string json = File.ReadAllText(itable);
                Debug.Log(json);
                idataListWrapper wrapper = JsonUtility.FromJson<idataListWrapper>(json);

                if (wrapper == null)
                {
                    Debug.LogError("idataListWrapper 파싱 실패: wrapper가 null입니다.");
                    return new idata(); // 혹은 null 반환도 가능
                }

                if (wrapper.list == null)
                {
                    Debug.LogError("idataListWrapper.list가 null입니다. JSON 구조를 확인하세요.");
                    return new idata();
                }

                idata selected = wrapper.list.Find(i => i.ID == id);

                if (selected == null)
                {
                    Debug.LogError($"id '{id}'에 해당하는 idata를 찾을 수 없습니다.");
                    return new idata();
                }

                Debug.Log("idata 정상 로드 완료");
                return selected;
            }
            else
            {
                Debug.LogWarning("idata.json 파일이 없습니다.");
                return new idata();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("idata 로딩 중 예외 발생: " + ex.Message);
            return new idata();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bool success = inven.instance.AddItem(Name);
            if (success)
                Destroy(gameObject);
        }
    }
}
