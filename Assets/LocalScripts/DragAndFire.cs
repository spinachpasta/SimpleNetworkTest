using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndFire : MonoBehaviour
{
    GameObject target;
    UnitScript targetScript;
    bool grabbing;
    Plane targetPlane = new Plane(Vector3.up, 0);
    Vector3 localOrigin;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "unit")
                {
                    target = hit.transform.gameObject;
                    targetScript = hit.transform.GetComponent<UnitScript>();
                    grabbing = true;
                    targetPlane = new Plane(Vector3.up, target.transform.position);
                    float enter=0;
                    targetPlane.Raycast(ray, out enter);
                    localOrigin = ray.GetPoint(enter);
                    if (!targetScript.owned)
                    {
                        grabbing = false;
                        target = null;
                        targetScript = null;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (grabbing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float enter = 0;
                targetPlane.Raycast(ray, out enter);
                Debug.Log(localOrigin - ray.GetPoint(enter));
                targetScript.rg.AddForce(localOrigin - ray.GetPoint(enter),ForceMode.Impulse);
                grabbing = false;
            }
        }
    }
}
