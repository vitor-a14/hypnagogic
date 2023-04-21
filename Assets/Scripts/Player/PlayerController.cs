using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    //Player controls config's
    [Header("Controls")]
    public float velocity;
    public float floorFriction;
    public LayerMask walkableLayers;
    public float groundDistanceCheck;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool onGround;

    [Range(1, 2)] public float runMultiplier;
    [HideInInspector] public bool aboveToggleSpeed;
    private float toggleSpeed = 1.5f;

    //Crouching
    [Header("Crouching")]
    public float standingHeight;
    public float crouchingHeight;
    public float crouchingMultiplier;
    [HideInInspector] public bool isCrounching = false;

    //Movement setup
    [HideInInspector] public Inputs controls; //Only this input is necessary, acess him from other scripts
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Vector3 processedDirection;
    [HideInInspector] public Vector2 input;
    [HideInInspector] public string floorType;
    private Transform cam;
    private Vector3 direction;
    private Vector3 surfaceNormal;
    private CapsuleCollider col;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        controls = new Inputs();
        controls.Enable();

        rigid = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        col = GetComponent<CapsuleCollider>();

        //Setup input callbacks
        controls.Player.Movement.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => input = Vector2.zero;
        controls.Player.Run.performed += ctx => TriggerRun();
        controls.Player.Crouch.performed += ctx => Crouch();
        controls.Player.UsePotion.performed += ctx => PlayerStatus.Instance.ConsumeSoulPotion();
        controls.Player.Inventory.performed += ctx => InventoryManager.Instance.ListItems();

        UseMouse(false);
    }

    private void Update()
    {
        CheckGround(); 

        //Input process
        Vector3 forward = Vector3.Cross(transform.up, -cam.right).normalized;
        Vector3 right = Vector3.Cross(transform.up, cam.forward).normalized;

        //Input treatment
        input = Vector2.ClampMagnitude(input, 1f);
        direction = (forward * input.y + right * input.x) * velocity;

        aboveToggleSpeed = rigid.velocity.magnitude > toggleSpeed; //Check if the player is not sticked in a wall

        if(!aboveToggleSpeed)
            isRunning = false;

        if (onGround) //If it is on floor, apply friction and get the surface normal for movement
        {
            rigid.drag = floorFriction;
            processedDirection = Vector3.ProjectOnPlane(direction, surfaceNormal);
        }
        else //If is not on floor, doesn't apply friction and move normally
        {
            rigid.drag = 0f;
            processedDirection = direction;
        }
    }

    private void TriggerRun()
    {
        //If the player is crouching, try to stand up
        if(isCrounching)
        {
            Crouch();
            if(isCrounching) return; //If the player continues crouching, do not start the run logic
        }

        isRunning = PlayerStatus.Instance.currentStamina > 0 && aboveToggleSpeed;
        PlayerStatus.Instance.staminaRecoveryTimer = 0;

        if(CombatHandler.Instance.defending || CombatHandler.Instance.attacking) isRunning = false;
    }

    private void FixedUpdate() 
    {
        //Movement apply
        if(!DialogueManager.Instance.dialogueIsPlaying && !InventoryManager.Instance.onInventory) 
        {
            if(isRunning) 
            {
                processedDirection.x *= runMultiplier;
                processedDirection.z *= runMultiplier;
            }
            else if(isCrounching || CombatHandler.Instance.attacking || CombatHandler.Instance.defending)
            {
                processedDirection.x *= crouchingMultiplier;
                processedDirection.z *= crouchingMultiplier;
            }

            rigid.velocity = new Vector3(processedDirection.x, rigid.velocity.y, processedDirection.z);
        }

        //Gravity logic
        Vector3 gravity = Physics.gravity.y * 5f * Vector3.up;
        rigid.AddForce(gravity, ForceMode.Acceleration);
    }

    //Check if player is on floor and get the surface normal to normalize movement in all angles
    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.1f, -transform.up, out hit, groundDistanceCheck, walkableLayers))
        {
            surfaceNormal = hit.normal;
            floorType = hit.transform.tag;
            onGround = true;
        } 
        else
        {
            onGround = false;
        }
    }

    private void Crouch()
    {
        //If the player is already crouching, check if he can stand up
        if(isCrounching && Physics.Raycast(transform.position, transform.up, 1.5f, walkableLayers))
            return;

        isCrounching = !isCrounching;

        if(isCrounching)
        {
            col.height = crouchingHeight;
            isRunning = false;
        }
        else
            col.height = standingHeight;
    }

    public void UseMouse(bool state) 
    {
        Cursor.visible = state;

        if(state)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
