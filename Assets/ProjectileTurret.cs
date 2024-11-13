using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileTurret : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 1;
    [SerializeField] Vector3 gravity = new Vector3(0, -9.8f, 0);
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;
    [SerializeField] bool useLowAngle;
    Ray cameraRay;

    List<Vector3> points = new List<Vector3>();
    Vector3 newPoint;
    [SerializeField] float distance = 1000f;

    GameObject surface;

    // Start is called before the first frame update
    //void Start()
    //

    //}

    // Update is called once per frame
    void Update()
    {
        points.Clear();
        points.Add(barrelEnd.position);

        if (Input.GetButtonDown("Fire1"))
            Fire();

        TrackMouse();
        TurnBase();
        RotateGun();

        if (Physics.Raycast(barrelEnd.position, barrelEnd.forward, out RaycastHit hit, distance, targetLayer))
        {
            newPoint = new Vector3(hit.point.x, crosshair.transform.position.y, hit.point.z);
            points.Add(newPoint);

            line.positionCount = points.Count;
            for (int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, points[i]);
            }
        }



    }

    void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, barrelEnd.position, gun.transform.rotation);
        projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * barrelEnd.transform.forward;
        projectile.GetComponent<Projectile>().lifeTime(5.0f);
    }

    void TrackMouse()
    {

        if(Input.mousePosition != null)
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(cameraRay.direction != null)
        {
            if (Physics.Raycast(cameraRay, out hit, 1000, targetLayer))
            {
                crosshair.transform.forward = hit.normal;
                crosshair.transform.position = hit.point + hit.normal * 0.1f;
            }
        }

    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }

    void RotateGun()
    {
        float? angle = CalculateTrajectory(crosshair.transform.position, useLowAngle);
        if (angle != null)
            gun.transform.localEulerAngles = new Vector3(360f - (float)angle, 0, 0);
    }

    float? CalculateTrajectory(Vector3 target, bool useLow)
    {
        Vector3 targetDir = target - barrelEnd.position;
        
        float y = targetDir.y;
        targetDir.y = 0;

        float x = targetDir.magnitude;

        float v = projectileSpeed;
        float v2 = Mathf.Pow(v, 2);
        float v4 = Mathf.Pow(v, 4);
        float g = gravity.y;
        float x2 = Mathf.Pow(x, 2);

        float underRoot = v4 - g * ((g * x2) + (2 * y * v2));

        if (underRoot >= 0)
        {
            float root = Mathf.Sqrt(underRoot);
            float highAngle = v2 + root;
            float lowAngle = v2 - root;

            if (useLow)
                return (Mathf.Atan2(lowAngle, g * x) * Mathf.Rad2Deg);
            else
                return (Mathf.Atan2(highAngle, g * x) * Mathf.Rad2Deg);
        }
        else
            return null;
    }
}
