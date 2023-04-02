using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance; //Acess from other scripts

    //Player controls config's
    public float velocity;
    public float floorFriction;
    public LayerMask walkableLayers;
    public float groundDistanceCheck;
    [HideInInspector] public bool onGround;

    //Movement setup
    [HideInInspector] public Inputs controls; //Only this input is necessary, acess him from other scripts
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Vector3 processedDirection;
    [HideInInspector] public Vector2 input;
    Transform cam;
    Vector3 direction;
    Vector3 surfaceNormal;

    void Awake()
    {
        Instance = this;
        controls = new Inputs();
        controls.Enable();
    }

    private void Start() 
    {
        rigid = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        //Setup input callbacks
        controls.Player.Movement.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => input = Vector2.zero;
        controls.Player.Inventory.performed += ctx => HandleInventory();

        UseMouse(false);
    }

    void Update()
    {
        //Input process
        Vector3 forward = Vector3.Cross(transform.up, -cam.right).normalized;
        Vector3 right = Vector3.Cross(transform.up, cam.forward).normalized;

        //Input treatment
        input = Vector2.ClampMagnitude(input, 1f);
        direction = (forward * input.y + right * input.x) * velocity;
    }

    void FixedUpdate() 
    {
        CheckGround(); 

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

        //Movement apply
        if(!DialogueManager.Instance.dialogueIsPlaying && !InventoryManager.Instance.onInventory) 
            rigid.velocity = new Vector3(processedDirection.x, rigid.velocity.y, processedDirection.z);

        //Gravity logic
        Vector3 gravity = Physics.gravity.y * 5f * Vector3.up;
        rigid.AddForce(gravity, ForceMode.Acceleration);
    }

    private void HandleInventory()
    {
        InventoryManager.Instance.ListItems();
    }

    //Check if player is on floor and get the surface normal to normalize movement in all angles
    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.1f, -transform.up, out hit, groundDistanceCheck, walkableLayers))
        {
            surfaceNormal = hit.normal;
            onGround = true;
        } 
        else
        {
            onGround = false;
        }
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
