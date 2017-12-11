using UnityEngine;
using UnityEditor.Animations;

[RequireComponent(typeof(Animator))]
public class Recorder : MonoBehaviour
{

    public enum Action
    {
        NORMAL_MOVEMENT, // using transforms, physics and nav mesh
        USE_ANIMATION, // using an animation
        RECORD_MOVEMENT // record the normal movement into an animation
    }

    public Action action;
    public AnimatorController animController;
    public float animationSpeed = 1;
    public int frameRate = 25;
    public int selectedFrame = 1;
    public int currentFrame = 0;

    [HideInInspector]
    public int numberOfFrames = 0;

    private UnityEditor.Experimental.Animations.GameObjectRecorder recorder;
    private Transform[] childs;
    private Animator animator;
    private bool moveToSelectedFrame, movedFrame;

    void Awake()
    {
        childs = GetComponentsInChildren<Transform>();
        animator = gameObject.GetComponent<Animator>();
        switch (action)
        {

            case Action.RECORD_MOVEMENT:
                recorder = new UnityEditor.Experimental.Animations.GameObjectRecorder();
                animator.enabled = false;
                break;

            case Action.NORMAL_MOVEMENT:
                animator.enabled = false;
                break;

            case Action.USE_ANIMATION:
                animator.enabled = true;
                UnableComponents();
                animator.runtimeAnimatorController = animController;

                moveToSelectedFrame = false;
                movedFrame = false;
                break;
        }
    }

    void Start()
    {
        

        if (action == Action.RECORD_MOVEMENT)
        {
            recorder.root = gameObject;
            // Set it up to record the transforms recursively.
            recorder.BindComponent<Transform>(gameObject, true);
        }
    }


    private void Update()
    {
        if (action == Action.USE_ANIMATION)
        {
            currentFrame = Mathf.RoundToInt(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * animController.animationClips[0].length * frameRate);
            if (movedFrame)
            {
                movedFrame = false;
                animationSpeed = 0f;
            }
            else if (moveToSelectedFrame)
            {
                float selectedTime = (float)selectedFrame / frameRate;
                float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime * animController.animationClips[0].length;

                animationSpeed = (selectedTime - currentTime) / Time.deltaTime;
                movedFrame = true;
                moveToSelectedFrame = false;
            }

            animator.SetFloat("speed", animationSpeed);

        }
    }

    void LateUpdate()
    {
        if (action == Action.RECORD_MOVEMENT)
        {
            recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    private void OnApplicationQuit()
    {
        if (action == Action.RECORD_MOVEMENT)
        {
            Debug.Log("Saveing...");
            recorder.SaveToClip((AnimationClip)animController.layers[0].stateMachine.defaultState.motion);
            recorder.ResetRecording();
            //UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("Saved");
        }
    }

    private void UnableComponents()
    {
        for (int i = 1; i < childs.Length; ++i) //first transform is this object
        {
            Collider[] colliders = childs[i].GetComponents<Collider>();
            foreach (Collider c in colliders)
                c.enabled = false;

            Behaviour[] components = childs[i].GetComponents<Behaviour>();
            foreach (Behaviour c in components)
                c.enabled = false;

        }
    }

    public float PlaybackDeltaTime
    {
        get
        {

            if (action == Action.USE_ANIMATION)
            {
                return Time.deltaTime * animationSpeed;
                //if (animator.playbackTime <= animator.recorderStartTime && animationSpeed < 0) return 0f;
                //else if (animator.playbackTime >= animator.recorderStopTime && animationSpeed > 0) return 0f;
                //else return Time.deltaTime * animationSpeed;
            }
            else return Time.deltaTime;
        }
    }

    public void MoveToSelectedFrame()
    {
        moveToSelectedFrame = true;
    }
}
