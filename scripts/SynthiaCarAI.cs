using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class SynthiaCarAI : MonoBehaviour
{

    [SerializeField] private UnityStandardAssets.Utility.WaypointCircuit circuit;
    [SerializeField] private AnimationCurve CurveAngle_DesiredMaximumSpeed;
    [SerializeField] private float MaxCurveSpeed = 50f;
    [SerializeField] private float MinCurveSpeed = 20f;
    [SerializeField] private AnimationCurve SpeedError_BrakeValue;
    [SerializeField] private float MaximumSpeedError = 10f;

    [SerializeField] private float minDetectionRange = 5f;
    [SerializeField] [Range(0,89)] private float detectionAngle = 30f;
    private float detectionRange;
    
    private WaypointTracker traker;
    private CarController controller;
	public GameObject rayPoint;

    private Ray[] rays;
    private bool stop;
	public Transform carpath;

    void Start()
    {
		UnityStandardAssets.Utility.WaypointCircuit cir = (UnityStandardAssets.Utility.WaypointCircuit)Instantiate (circuit, carpath);


        GameObject target = new GameObject();
        target.transform.parent = transform;
        traker = new WaypointTracker(transform, cir, target.transform);
        detectionRange = 0;
        controller = GetComponent<CarController>();

        rays = new Ray[3];

        this.tag = "Vehicle";
    }

    // Update is called once per frame
    void Update()
    {
        stop = false;
		traker.Run();

        Vector3 targetPosition = traker.target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;

        float side = Vector3.Cross(transform.forward, direction).y;
        float angle = 0;

        if (side != 0)
        {
            angle = Vector3.Angle(transform.forward, direction) / controller.MaxSteeringAngle;
            if (angle > 1f) angle = 1.0f;

            if (side < 0) angle = -angle;
        }

        UpdateRange(); 
        LookForward();
        
        if (!stop)
        {
            float accel, brake;
            GetMovement(angle, out accel, out brake);

            controller.Move(angle, accel, -brake, 0f);
        }
        else
        {
            controller.Move(angle, 0f, -1f, -1f);
        }
    }
    
    private Vector3 RotateVector(Vector3 vector, float angle)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * detectionRange;
        float z = Mathf.Cos(Mathf.Deg2Rad * angle) * detectionRange;
        Vector3 targetVector = new Vector3(x, 0f, z);
        return transform.TransformVector(targetVector);
    }


    private void GetMovement(float angle, out float accel, out float brake)
    {
        accel = 0.0f;
        brake = 0.0f;

        if (controller.CurrentSpeed < MinCurveSpeed || angle < 0.05f)
        {
            accel = 1.0f;
            return;
        }

        angle = Mathf.Abs(angle);
        float speed_y = CurveAngle_DesiredMaximumSpeed.Evaluate(angle);

        float desiredSpeed = speed_y * (MaxCurveSpeed - MinCurveSpeed) + MinCurveSpeed;

        float currentSpeedError = controller.CurrentSpeed - desiredSpeed;

        if (currentSpeedError < 0) accel = speed_y;
        else if (currentSpeedError == 0) return;
        else
        {
            accel = 0.0f;
            float brake_x = currentSpeedError / MaximumSpeedError;
            brake = SpeedError_BrakeValue.Evaluate(brake_x);
            if (brake < 0.01f) brake = 0f;
        }


    }

    private void UpdateRange()
    {
        detectionRange = (minDetectionRange / 30f) * controller.CurrentSpeed;
        if (detectionRange < 3f) detectionRange = 3f;
    }

    private void LookForward()
    {
		rays[0] = new Ray(rayPoint.transform.position/* + Vector3.up*/, rayPoint.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(rays[0], detectionRange);
        for (int i = 0; i < hits.Length; ++i)
        {
			if (!hits[i].transform.Equals(rayPoint.transform))
            {
                Evaluate(hits[i].transform);
            }
        }

		rays[1] = new Ray(rayPoint.transform.position/* + Vector3.up*/, RotateVector(rayPoint.transform.forward, detectionAngle));
        hits = Physics.RaycastAll(rays[1], detectionRange);
        for (int i = 0; i < hits.Length; ++i)
        {
			if (!hits[i].transform.Equals(rayPoint.transform))
            {
                Evaluate(hits[i].transform);
            }
        }

		rays[2] = new Ray(rayPoint.transform.position/* + Vector3.up*/, RotateVector(rayPoint.transform.forward, -detectionAngle));
        hits = Physics.RaycastAll(rays[2], detectionRange);
        for (int i = 0; i < hits.Length; ++i)
        {
			if (!hits[i].transform.Equals(rayPoint.transform))
            {
                Evaluate(hits[i].transform);
            }
        }
    }

    private void Evaluate(Transform detectedObj)
    {
        if ((detectedObj.CompareTag("TrafficLight") && detectedObj.GetComponent<TrafficLight>().State == TrafficLight.TrafficLightState.RED)
            || detectedObj.CompareTag("Vehicle") 
            || detectedObj.CompareTag("Player")) 
            stop = true;
    }

    private void OnDrawGizmos()
    {
        if (traker != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, traker.target.position);
			Gizmos.DrawSphere (traker.target.position, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (rays[0].origin, 0.2f);
        }

        if (rays != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < rays.Length; ++i)
            {
                Gizmos.DrawLine(rays[i].origin, rays[i].origin + rays[i].direction*detectionRange);
            }
        }
    }
}
