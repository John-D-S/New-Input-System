using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Transform CamTransform => camera.transform;

    [Header("Movement Settings")]
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float walkSpeedModifier = 1f;
    [SerializeField] private float sprintSpeedModifier = 2f;
    [SerializeField] private float crouchSpeedModifier = .5f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;


    [Header("Look Settings")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private new Camera camera;
    //this number is set to a low number because our camera controller
    // is not framerate locked which makes it feel smoother
    [SerializeField, Range(0, 3)] private float sensitivity = .5f;
    // this is how far up and down the camera will be able to look
    // 90 means full vertical look without inverting the camera
    [SerializeField, Range(0, 90)] private float verticalLookCap = 90f;
    [SerializeField] private float cameraSmoothing = 1;

    private new CapsuleCollider collider;
    private new Rigidbody rigidbody;

    // the current rotation of the camera that gets updated every time the input is changed.
    private Vector2 rotation = Vector2.zero;
    //the amount of movement that is wiating to be applied
    private Vector3 movement = Vector3.zero;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<CapsuleCollider>();
        rigidbody = gameObject.GetComponent<Rigidbody>();

        // This is how you link the funciton into the actual press/usage
        // of the action such as moving the mouse will perform the lookAction.
        lookAction.action.performed += OnLookPerformed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CamTransform.localRotation = Quaternion.AngleAxis(rotation.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);

        //apply the movement and reset it to 0
        UpdateMovement();
        transform.position += movement;
        movement = Vector3.zero;
    }

    private void UpdateMovement()
    {
        Vector3 camEuler = CamTransform.eulerAngles;
        
        float speed = defaultSpeed;
        if (crouchAction.action.ReadValue<float>() > 0)
        {
            speed *= crouchSpeedModifier;
        }
        if (sprintAction.action.ReadValue<float>() > 0)
        {
            speed *= sprintSpeedModifier;
        }
        

        Vector2 value = moveAction.action.ReadValue<Vector2>();
        movement += transform.forward * value.y * speed * Time.deltaTime;
        movement += transform.right * value.x * speed * Time.deltaTime;
    }

    private void OnLookPerformed(InputAction.CallbackContext _context)
    {
        //there has been some sort of input update

        //get the actual value of the input
        Vector2 value = _context.ReadValue<Vector2>();

        rotation.x += value.x * sensitivity;
        rotation.y += value.y * sensitivity;

        //prevent the vertical look from going outside the specified angle
        rotation.y = Mathf.Clamp(rotation.y, -verticalLookCap, verticalLookCap);
    }
}
