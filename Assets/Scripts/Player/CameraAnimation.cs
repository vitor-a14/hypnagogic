using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public static CameraAnimation Instance;
    [SerializeField] private Animator anim;
    private int currentState;

    public static readonly int idle = Animator.StringToHash("Idle");
    public static readonly int death = Animator.StringToHash("Death");

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");
    }

    private void Start() 
    {
        ChangeAnimation(idle);
    }

    public void ChangeAnimation(int state)
    {
        if(state == currentState) return;
        anim.CrossFade(state, 0, 0);
        currentState = state;
    }
}
