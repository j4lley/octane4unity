
using UnityEngine;

public class TrafficLightGroup : MonoBehaviour
{

    private TrafficLight[] trafficLights;
    private int it;
    private float elapsedTime;

    /// <summary>
    /// Sync the time of the traffic lights with the time of the animation running into Recorder object. Use Unity time if null.
    /// </summary>
    public Recorder syncWithAnimation;

    void Start()
    {
        it = 0;
        elapsedTime = 0;
        trafficLights = GetComponentsInChildren<TrafficLight>();
        foreach (TrafficLight light in trafficLights) light.syncWithAnimation = syncWithAnimation;
        trafficLights[0].NextState(); //put the first in green
    }


    void Update()
    {
        switch (trafficLights[it].State)
        {
            case TrafficLight.TrafficLightState.GREEN:
                if (syncWithAnimation == null) elapsedTime += Time.deltaTime;
                else elapsedTime += syncWithAnimation.PlaybackDeltaTime;

                if (elapsedTime >= trafficLights[it].greenTime)
                {
                    trafficLights[it].NextState();
                    elapsedTime = elapsedTime - trafficLights[it].greenTime;
                }
                else if (elapsedTime < 0)
                {
                    trafficLights[it].PreviousState();
                    //elapsedTime = trafficLights[it].greenTime + elapsedTime;
                }
                break;

            case TrafficLight.TrafficLightState.RED:
                if (elapsedTime < 0)
                {
                    --it;
                    if (it < 0) it = trafficLights.Length - 1;
                    elapsedTime = trafficLights[it].yellowTime + elapsedTime;
                    trafficLights[it].PreviousState();
                }
                else
                {
                    //elapsedTime = elapsedTime - trafficLights[it].yellowTime;
                    ++it;
                    if (it >= trafficLights.Length) it = 0;
                    trafficLights[it].NextState();
                }
                break;

            case TrafficLight.TrafficLightState.YELLOW:
                if (!syncWithAnimation) elapsedTime += Time.deltaTime;
                else elapsedTime += syncWithAnimation.PlaybackDeltaTime;

                if (elapsedTime > trafficLights[it].yellowTime)
                {
                    trafficLights[it].NextState();
                    elapsedTime = elapsedTime - trafficLights[it].yellowTime;
                }
                else if (elapsedTime < 0)
                {
                    trafficLights[it].PreviousState();
                    elapsedTime = elapsedTime + trafficLights[it].greenTime; 
                }
                break;

        }
    }
}
