using UnityEngine;

public class TrafficLight : MonoBehaviour {

    public enum TrafficLightState
    {
        RED = 0,
        GREEN = 1,
        YELLOW = 2,
        LENGHT
    }

	private TrafficLightState state = TrafficLightState.RED;
	public trafficLightController[] TFcontrollers;

    public float greenTime = 5f;
    public float yellowTime = 1f;

    [HideInInspector]
    public Recorder syncWithAnimation;

    private float elapsedTime;
    private BoxCollider collider;

    private void Awake()
    {
        this.tag = "TrafficLight";
    }

    void Start () {
        state = TrafficLightState.RED;
        if (yellowTime < 0) yellowTime = 0;
        elapsedTime = 0;
        collider = GetComponent<BoxCollider>();
		foreach(trafficLightController cont in TFcontrollers){
            cont.InitState();
        }
	}

    void Update()
    {
        //if (state == TrafficLightState.YELLOW)
        //{
        //    if (!syncWithAnimation) elapsedTime += Time.deltaTime;
        //    else elapsedTime += syncWithAnimation.PlaybackDeltaTime;
        //    if (elapsedTime > yellowTime)
        //    {
        //        NextState();
        //    }
        //    else if (elapsedTime < 0)
        //    {
        //        PreviousState();
        //    }
        //}
    }

    public void NextState()
    {
        elapsedTime = 0;
        switch (state)
        {
            case TrafficLightState.RED:
                state = TrafficLightState.GREEN;
                break;
            case TrafficLightState.YELLOW:
                state = TrafficLightState.RED;
                break;
            case TrafficLightState.GREEN:
                state = TrafficLightState.YELLOW;
                break;
        }

		foreach(trafficLightController cont in TFcontrollers){
            if ((int)state == 0)
                cont.changeState((int)TrafficLightState.LENGHT - 1, (int)state);
            else
                cont.changeState((int)state - 1, (int)state);
		}
    }

    public void PreviousState()
    {
        switch (state)
        {
            case TrafficLightState.RED:
                state = TrafficLightState.YELLOW;
                elapsedTime = yellowTime;
                break;
            case TrafficLightState.YELLOW:
                state = TrafficLightState.GREEN;
                elapsedTime = greenTime;

                break;
            case TrafficLightState.GREEN:
                state = TrafficLightState.RED;
                break;
        }

        foreach (trafficLightController cont in TFcontrollers)
        {
            if ((int)state == (int)TrafficLightState.LENGHT -1)
                cont.changeState(0, (int)state);
            else
                cont.changeState((int)state + 1, (int)state);
        }
    }

    public TrafficLightState State
    {
        get
        {
            return state;
        }
    }

    private void OnDrawGizmos()
    {
        switch (state)
        {
            case TrafficLightState.RED:
                Gizmos.color = Color.red;
                break;
            case TrafficLightState.GREEN:
                Gizmos.color = Color.green;
                break;
            case TrafficLightState.YELLOW:
                Gizmos.color = Color.yellow;
                break;
        }

        if (collider != null)
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}
