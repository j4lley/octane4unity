using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DemoCamera : MonoBehaviour
{
    //private Camera c;
    //private Vector3 direction = Vector3.right;

    //public float distanceToCenter;
    public float velocity;
    //public float pitch;
    public Vector3 lookAt;

	void Start ()
    {
        //c = GetComponent<Camera>();
        transform.LookAt(lookAt);
    }

    void Update ()
    {
        transform.RotateAround(lookAt, new Vector3(0.0f, 1.0f, 0.0f), 2 * Time.deltaTime * -velocity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, lookAt);
        Gizmos.DrawSphere(lookAt, 0.3f);
    }
}
