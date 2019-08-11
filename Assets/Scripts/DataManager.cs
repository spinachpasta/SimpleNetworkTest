using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


//manages units
public class DataManager : MonoBehaviour
{
    public int myid;
    public int order;
    public List<Unit> units;
    public List<GameObject> prefabs;
    public List<UnitScript> instances;//gameObjects in the field
    public bool started;
    public bool serverOnly;

    public List<int> participants;
    // Start is called before the first frame update
    void Awake()
    {
        participants = new List<int>();
        myid = Random.Range(0, int.MaxValue);
        units = new List<Unit>();
        instances = new List<UnitScript>();
        //test for client
        if (!serverOnly)
        {
            participants.Add(myid);
            for (int i = 0; i < 10; i++)
            {
                Unit u = new Unit();
                u.owner = 0;
                //u.x = i+ Random.Range(0,3);
                u.z = 10 - i;
                u.uuid = (int)Random.Range(0, int.MaxValue);
                u.owner = myid;
                units.Add(u);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUnits();
        if (participants.Count >= 2&&!started)
        {
            startUntil -= Time.deltaTime;
            if (startUntil < 0)
            {
                started = true;
            }
        }
        //UpdateInstances();
        if (requestupdate)
        {
            requestupdate = false;
            UpdateInstances();
        }
    }
    bool requestupdate = false;
    float startUntil=3f;
    void UpdateInstances()
    {

        if (serverOnly) return;
        //removes instances
        for (int i = 0; i < instances.Count; i++)
        {

        }
        for (int i = 0; i < units.Count; i++)
        {
            bool found = false;
            for (int n = 0; n < instances.Count; n++)
            {
                if (instances[n].uuid == units[i].uuid)
                {
                    found = true;
                    instances[n].ApplyData(units[i]);
                    break;
                }
            }
            if (!found)
            {
                GameObject g = Instantiate(prefabs[units[i].type],new Vector3(units[i].x,0,units[i].z),Quaternion.identity);
                UnitScript unitScript = g.GetComponent<UnitScript>();
                unitScript.ApplyData(units[i]);
                unitScript.setOwned(myid == units[i].owner);
                instances.Add(unitScript);
            }
        }
    }
    void UpdateUnits()
    {
        for(int i = 0; i < instances.Count; i++)
        {
            if (!instances[i].owned) continue;
            for(int j = 0; j < units.Count; j++)
            {
                if (units[j].uuid == instances[i].uuid)
                {
                    units[j].Apply(instances[i]);
                    break;
                }
            }
        }
    }
    bool firstUpdate = false;
    public void UpdateData(List<Unit> us)
    {
        requestupdate = true;
        for (int i = 0; i < us.Count; i++)
        {
            if (us[i].owner != myid)
            {
                bool found = false;
                for (int n = 0; n < units.Count; n++)
                {
                    if (units[n].uuid == us[i].uuid)
                    {
                        found = true;
                        units[n] = us[i];
                        break;
                    }
                }
                if (!found)
                {
                    units.Add(us[i]);
                }
            }
        }
        if (!started)
        {
            float symmetry = -order * 2 + 1;
            List<Unit> mine = new List<Unit>();
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].owner == myid)
                {
                    units[i].x = symmetry * 5;
                }
            }
            firstUpdate = false;
        }
    }
}

[System.Serializable]
public class Unit
{
    public int owner;
    public int id;
    public int uuid;
    public int type;
    public float x;
    public float z;
    public float vx;
    public float vz;
    public float av;//angular velocity

    public void Apply(UnitScript u)
    {
        x = u.transform.position.x;
        z = u.transform.position.z;
        vx = u.rg.velocity.x;
        vz = u.rg.velocity.z;
        av = u.rg.angularVelocity.y;
    }
}
