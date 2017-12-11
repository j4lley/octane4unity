using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trafficLightPed : MonoBehaviour {


	public TrafficLight[] tfs;
	public GameObject greenLight;
	public GameObject redLight;

	// Use this for initialization
	void Start () {
        //greenLight.SetActive (true);
        //redLight.SetActive (false);
        bool isgreen = true;
        foreach (TrafficLight tf in tfs)
        {
            if (tf.State != TrafficLight.TrafficLightState.GREEN)
            {
                isgreen = false;
            }
        }

        if (isgreen)
        {
            redLight.transform.Translate(-Vector3.up * 1000f);
        }
        else if (!isgreen)
        {
            greenLight.transform.Translate(-Vector3.up * 1000f);
        }
    }

	// Update is called once per frame
	void Update () {
		bool isgreen = true;
		foreach(TrafficLight tf in tfs){
			if(tf.State != TrafficLight.TrafficLightState.GREEN){
                isgreen = false;
			}
		}

		if (isgreen && greenLight.transform.position.y < redLight.transform.position.y) {
            greenLight.transform.Translate(Vector3.up * 1000f);
            redLight.transform.Translate(-Vector3.up * 1000f);
		} else if (!isgreen && greenLight.transform.position.y > redLight.transform.position.y) {
            greenLight.transform.Translate(-Vector3.up * 1000f);
            redLight.transform.Translate(Vector3.up * 1000f);
        }


	}

    private void changeState()
    {
        
    }
}
