using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
public class mdata
{
    public string id;
    public string name;
    public float hp;
    public float atk;
    public float def;
    public float move_speed;
    public float attack_speed;
    public string base_weapon;
    public string default_activeslot;
    public string default_passiveslot;
    public float drop_EXP;
    public float craft_time;
}

[System.Serializable]
public class mdataListWrapper
{
    public List<mdata> list;

}
public class monster : MonoBehaviour
{
    public string mtable;
    public string Name;
    public float hp;
    public float atk;
    public float def;
    public float movespeed;
    public float attackspeed;
    public float dropEXP;
    public float crafttime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mtable = Path.Combine(Application.streamingAssetsPath, "json/mstats.json");

        mdata selected = loadmon("M_M_001");
        Name = selected.name;
        hp = selected.hp;
        atk = selected.atk;
        def = selected.def;
        movespeed = selected.move_speed;
        attackspeed = selected.attack_speed;
        dropEXP = selected.drop_EXP;
        crafttime = selected.craft_time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    mdata loadmon(string id)
    {
        try
        {
            if (File.Exists(mtable))
            {
                string json = File.ReadAllText(mtable);
                Debug.Log(json);
                mdataListWrapper wrapper = JsonUtility.FromJson<mdataListWrapper>(json);

                if (wrapper == null)
                {
                    Debug.LogError("mdataListWrapper 파싱 실패: wrapper가 null입니다.");
                    return new mdata(); // 혹은 null 반환도 가능
                }

                if (wrapper.list == null)
                {
                    Debug.LogError("mdataListWrapper.list가 null입니다. JSON 구조를 확인하세요.");
                    return new mdata();
                }

                mdata selected = wrapper.list.Find(m => m.id == id);

                if (selected == null)
                {
                    Debug.LogError($"id '{id}'에 해당하는 mdata를 찾을 수 없습니다.");
                    return new mdata();
                }

                Debug.Log("mdata 정상 로드 완료");
                return selected;
            }
            else
            {
                Debug.LogWarning("mdata.json 파일이 없습니다.");
                return new mdata();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("mdata 로딩 중 예외 발생: " + ex.Message);
            return new mdata();
        }
    }
}

