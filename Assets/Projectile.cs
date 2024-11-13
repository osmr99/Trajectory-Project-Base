using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        //Destroy(gameObject, 5.0f);
    }

    public void lifeTime(float secs)
    {
        Destroy(gameObject, secs);
    }
}
