using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class pedestrianAI : MonoBehaviour {


	public bool reverse = false;
	public enum pedSTATE { WALK, STOP};
	public pedSTATE state = pedSTATE.WALK;

	private Animator characterAnimator;
	private UnityEngine.AI.NavMeshAgent characterNavMeshAgent;
	private Vector3 nextPoint;
	public GameObject pathObject;
	private Transform[] pathNodes;
	private int actualNode = 1;
	Vector2 v2Pedestrian;
	Vector2 v2Point;


	// Use this for initialization
	void Start () {
		pathNodes = pathObject.GetComponentsInChildren<Transform> ().Where (go => go.gameObject != this.gameObject).ToArray();
	
		if(reverse){ pathNodes = pathNodes.Reverse().ToArray();}

		characterAnimator = GetComponent<Animator> ();
		characterNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		getFirstPoint ();
		if (pathNodes.Length > 1) {
			nextPoint = pathNodes [actualNode].position;
		} else {
			nextPoint = transform.position;
		}
		v2Point.x = nextPoint.x;
		v2Point.y = nextPoint.z;
		characterNavMeshAgent.SetDestination (nextPoint);
		characterAnimator.SetFloat ("speed", 2.0f);
	}

	// Update is called once per frame
	void Update () {
		if(state == pedSTATE.WALK){
			v2Pedestrian.x = transform.position.x;
			v2Pedestrian.y = transform.position.z;

			if(Vector2.Distance(v2Pedestrian, v2Point)<0.30f){
				if (actualNode < pathNodes.Length - 1) {
					actualNode++;
					nextPoint = pathNodes [actualNode].position;
					v2Point.x = nextPoint.x;
					v2Point.y = nextPoint.z;
					characterNavMeshAgent.SetDestination (nextPoint);
				} else {
					characterAnimator.SetFloat ("speed", 0.0f);
					characterNavMeshAgent.SetDestination (transform.position);
					state = pedSTATE.STOP;
				}
			}
		}
	}


	void getFirstPoint(){
		v2Pedestrian.x = transform.position.x;
		v2Pedestrian.y = transform.position.z;
		v2Point.x = pathNodes [actualNode].position.x;
		v2Point.y = pathNodes [actualNode].position.z;
		float nearPointDistance = Vector2.Distance(v2Pedestrian, v2Point);

		for(int i=1;i<pathNodes.Length;i++){
			v2Point.x = pathNodes [i].position.x;
			v2Point.y = pathNodes [i].position.z;
			if(Vector2.Distance(v2Pedestrian, v2Point)< nearPointDistance){
				nearPointDistance = Vector2.Distance(v2Pedestrian, v2Point);
				actualNode = i;
			}
		}
	}


	/*public void changeSate(pedSTATE newState){
		state = newState;
	}*/

	/*void OnTriggerEnter(Collider col){
		crosswalk cw = col.gameObject.GetComponent<crosswalk> ();
		if (cw != null) {
			if (cw.crosswalkState == crosswalk.STATE.RED) {
				state = pedSTATE.STOP;
				characterAnimator.SetFloat ("speed", 0.0f);
				characterNavMeshAgent.SetDestination (transform.position);
			}
			if (cw.crosswalkState == crosswalk.STATE.GREEN) {
				state = pedSTATE.WALK;
				characterAnimator.SetFloat ("speed", 2.0f);
				characterNavMeshAgent.SetDestination (nextPoint);
			}
		}
	}

	void OnTriggerStay(Collider col){
		crosswalk cw = col.gameObject.GetComponent<crosswalk> ();
		if (cw != null) {
			if (cw.crosswalkState == crosswalk.STATE.GREEN) {
				state = pedSTATE.WALK;
				characterAnimator.SetFloat ("speed", 2.0f);
				characterNavMeshAgent.SetDestination (nextPoint);
			}
		}
	}*/
}
