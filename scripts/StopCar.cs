using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class stopCar : MonoBehaviour {


	public enum carsState { STOP, RUN }; 

	public carsState state = carsState.STOP;

	private int actualframe=0;
	public int maxframe = 500;
	public bool change=false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (change) {
			if (Time.timeScale != 0) {
				actualframe++;
				if (actualframe >= maxframe) {
					if (state == carsState.STOP) {
						state = carsState.RUN;
					} else {
						state = carsState.STOP;
					}
					actualframe = 0;
				}
			}
		}
	}

	void OnTriggerEnter(Collider col){
		//if (state == carsState.STOP) {
			CarAIControl carAI = col.gameObject.GetComponent<CarAIControl> ();
			if (carAI != null) {
				if (state == carsState.STOP) {
					carAI.run = false;
				} else {
					carAI.run = true;
				}
			}
		//}
	}

	void OnTriggerStay(Collider col){
		//if (state == carsState.STOP) {
			CarAIControl carAI = col.gameObject.GetComponent<CarAIControl> ();
			if (carAI != null) {
				if (state == carsState.STOP) {
					carAI.run = false;
				} else {
					carAI.run = true;
				}
			}
		//}
	}

	void OnTriggerExit(Collider col){
		//if (state == carsState.STOP) {
			CarAIControl carAI = col.gameObject.GetComponent<CarAIControl> ();
			if (carAI != null) {
				carAI.run = true;
			}
		//}
	}
}
