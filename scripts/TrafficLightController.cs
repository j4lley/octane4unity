using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trafficLightController : MonoBehaviour
{


    public GameObject greenLight;
    public GameObject redLight;
    public GameObject amberLight;
    //public int timeGreen = 500;
    //public int timeAmber = 200;
    //public int timeRed = 700;
    //public int actualTime=0;
    //public TrafficLightState actualState = TrafficLightState.GREEN;
    //public enum TrafficLightState {GREEN, AMBER, RED};
    //public Color emisionGreen = Color.green;	
    //public Color emisionAmber = Color.yellow;
    //public Color emisionRed = Color.red;


    // Use this for initialization
    void Start()
    {
        /*if (actualState == TrafficLightState.GREEN) {
			activeState (TrafficLightState.GREEN);
			actualState = TrafficLightState.GREEN;
		}else if (actualState == TrafficLightState.RED) {
			activeState (TrafficLightState.RED);
			actualState = TrafficLightState.RED;
		}else if (actualState == TrafficLightState.AMBER) {
			activeState (TrafficLightState.AMBER);
			actualState = TrafficLightState.AMBER;
		}
		actualTime = 0;
*/

    }

    // Update is called once per frame
    void Update()
    {
        /*if (Time.timeScale != 0) {
			if (actualState == TrafficLightState.GREEN) {
				if (actualTime == timeGreen) {
					actualTime = 0;
					activeState (TrafficLightState.AMBER);
					actualState = TrafficLightState.AMBER;
				}
			} else if (actualState == TrafficLightState.AMBER) {
				if (actualTime == timeAmber) {
					actualTime = 0;
					activeState (TrafficLightState.RED);
					actualState = TrafficLightState.RED;
				}
			} else if (actualState == TrafficLightState.RED) {	
				if (actualTime == timeRed) {
					actualTime = 0;
					activeState (TrafficLightState.GREEN);
					actualState = TrafficLightState.GREEN;
				}
			}
			actualTime += 1;
		}*/
    }


    /*private void activeState(TrafficLightState newState){
		greenLight.GetComponent<Renderer> ().material.DisableKeyword ("_EMISSION");
		amberLight.GetComponent<Renderer> ().material.DisableKeyword ("_EMISSION");
		redLight.GetComponent<Renderer> ().material.DisableKeyword ("_EMISSION");

		if(actualState == TrafficLightState.GREEN){
			greenLight.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
			greenLight.GetComponent<Renderer> ().material.SetColor("_EmissionColor", emisionGreen);
		}else if(actualState == TrafficLightState.AMBER){
			amberLight.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
			amberLight.GetComponent<Renderer> ().material.SetColor("_EmissionColor", emisionAmber);
		} else 	if(actualState == TrafficLightState.RED){
			redLight.GetComponent<Renderer> ().material.EnableKeyword ("_EMISSION");
			redLight.GetComponent<Renderer> ().material.SetColor("_EmissionColor", emisionRed);
		}
	}*/


    /*
public void changeState(TrafficLight.TrafficLightState state){
    switch (state)
    {
        case TrafficLight.TrafficLightState.RED:
            greenLight.SetActive (false);
            amberLight.SetActive (false);
            redLight.SetActive (true);
            break;
        case TrafficLight.TrafficLightState.YELLOW:
            greenLight.SetActive (false);
            amberLight.SetActive (true);
            redLight.SetActive (false);
            break;
        case TrafficLight.TrafficLightState.GREEN:
            greenLight.SetActive (true);
            amberLight.SetActive (false);
            redLight.SetActive (false);
            break;
        default:
            break;
    }
}
*/

        /// <summary>
        /// Init the trafficlight in red
        /// </summary>
    public void InitState()
    {
        greenLight.transform.Translate(-Vector3.up * 1000f);
        amberLight.transform.Translate(-Vector3.up * 1000f);
        greenLight.SetActive(true);
        redLight.SetActive(true);
        amberLight.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from">Previous state (TrafficLightState)</param>
    /// <param name="to">Current state (TrafficLightState)</param>
    public void changeState(int from, int to)
    {

        switch ((TrafficLight.TrafficLightState)from)
        {
            case TrafficLight.TrafficLightState.GREEN:
                greenLight.transform.Translate(-Vector3.up * 1000f);
                break;
            case TrafficLight.TrafficLightState.YELLOW:
                amberLight.transform.Translate(-Vector3.up * 1000f);
                break;
            case TrafficLight.TrafficLightState.RED:
                redLight.transform.Translate(-Vector3.up * 1000f);
                break;
        }

        switch ((TrafficLight.TrafficLightState)to)
        {
            case TrafficLight.TrafficLightState.GREEN:
                greenLight.transform.Translate(Vector3.up * 1000f);
                break;
            case TrafficLight.TrafficLightState.YELLOW:
                amberLight.transform.Translate(Vector3.up * 1000f);
                break;
            case TrafficLight.TrafficLightState.RED:
                redLight.transform.Translate(Vector3.up * 1000f);
                break;
        }
    }

}
