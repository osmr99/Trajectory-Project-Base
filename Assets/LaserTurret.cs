using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserTurret : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;

    List<Vector3> laserPoints = new List<Vector3>();

    GameObject surface;
    Vector3 newPoint;

    float negX;
    float negY;
    float negZ;
    float negRotationX;
    float negRotationY;
    float negRotationZ;

    bool flipX = true;
    bool flipY = true;
    bool flipZ = true;
    //bool flipXtwo = false;
    //bool flipYtwo = false;
    //bool flipZtwo = false;

    // Start is called before the first frame update
    //void Start()
    //{
        
   // }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("flipXX"))
        //{
            //flipXtwo = !flipXtwo;
            //flipX = !flipX;
        //}

        //if (Input.GetButtonDown("flipYY"))
        //{
            //flipYtwo = !flipYtwo;
            //flipY = !flipY;
        //}

        //if (Input.GetButtonDown("flipZZ"))
        //{
            //flipZtwo = !flipZtwo;
            //flipZ = !flipZ;
        //}
            



        TrackMouse();
        TurnBase();

        laserPoints.Clear();
        laserPoints.Add(barrelEnd.position);

        if(Physics.Raycast(barrelEnd.position, barrelEnd.forward, out RaycastHit hit, 1000.0f, targetLayer))
        {
            laserPoints.Add(hit.point);
            if(surface != null)
            {
                if (flipX)
                {
                    negX = -hit.point.x * 6;
                    negRotationX = -hit.transform.rotation.x;
                }
                else
                {
                    negX = hit.point.x + hit.point.x + hit.point.x + hit.point.x + hit.point.x + hit.point.x;
                    negRotationX = hit.transform.rotation.x;
                }


                if (flipY && surface.transform.position.y != 0)
                {
                    negY = -hit.point.y * 6;
                    negRotationY = -hit.transform.rotation.y;
                }
                else if (!flipY && surface.transform.rotation.x == 0 && surface.transform.rotation.y == 0 && surface.transform.rotation.z == 0)
                {
                    negY = Mathf.Abs(hit.point.y * 1000000000000000002);
                    negRotationY = -hit.transform.rotation.y;
                }
                else
                {
                    negY = hit.point.y + hit.point.y + hit.point.y + hit.point.y + hit.point.y + hit.point.y;
                    negRotationY = hit.transform.rotation.y;
                }

                if (flipZ)
                {
                    negZ = -hit.point.z * 6;
                    negRotationZ = -hit.transform.rotation.z;
                }
                else
                {
                    negZ = hit.point.z + hit.point.z + hit.point.z + hit.point.z + hit.point.z + hit.point.z;
                    negRotationZ = hit.transform.rotation.z;
                }
            }



            if (surface != null)
            {
                newPoint = new Vector3(negX, negY, negZ);
                laserPoints.Add(newPoint);
            }
        }

        line.positionCount = laserPoints.Count;

        if(hit.collider != null)
        {
            surface = hit.collider.gameObject;

            if ((surface.gameObject.transform.rotation.x != 0 || surface.gameObject.transform.rotation.x != -0.7071058f) && surface.gameObject.transform.rotation.y <= 0)
                flipX = true;
            else
                flipX = false;

            if (surface.gameObject.transform.rotation.x < -0.9f || surface.gameObject.transform.rotation.y < 0)
                flipY = true;
            else
                flipY = false;

            if (surface.gameObject.transform.rotation.z <= 0)
                flipZ = true;
            else
                flipZ = false;
        }
        else
        {
            surface = null;
        }

        for(int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, laserPoints[i]);
        }


    }

    void TrackMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(cameraRay, out hit, 1000, targetLayer))
        {
            crosshair.transform.forward = hit.normal;
            crosshair.transform.position = hit.point + hit.normal * 0.1f;
        }
    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }
}
