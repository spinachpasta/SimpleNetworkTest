using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public int uuid;
    public bool owned;//is this unit owned by the player or not
    public Rigidbody rg;
    public int type;
    public Material red;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setOwned(bool b)
    {
        owned = b;
        if (!owned)
        {
            GetComponent<MeshRenderer>().sharedMaterial = red;
        }
    }
    public void ApplyData(Unit u)
    {
        type = u.type;
        uuid = u.uuid;
        if (owned) return;
        if(transform.position!= new Vector3(u.x, 0, u.z))
        {
            Debug.Log("an update occured!");
        }
        Debug.Log(new Vector3(u.x, 0, u.z));
        rg.position = new Vector3(u.x, 0, u.z);
        rg.velocity = new Vector3(u.vx, 0, u.vz);
    }
}
