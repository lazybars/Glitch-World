using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float skateSpeed = 12;
    public float gravity = -12;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    bool hasBoard = false;

    Animator animator;
    Transform cameraT;
    CharacterController controller;
    Rigidbody rig;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        rig = GetComponent<Rigidbody>();
    }

    void Update() {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero) {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        float targetSpeed = 0; 
        if (!hasBoard) {
            bool running = Input.GetKey(KeyCode.LeftShift);
            targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
            float animationSpeedPercent = ((running) ? 1 : .5f) * inputDir.magnitude;
            animator.SetFloat("SpeedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        } else {
            
            targetSpeed = skateSpeed * inputDir.magnitude;
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded) {
            velocityY = 0;
        }



        if (Input.GetKeyDown(KeyCode.Z)) {
            print("equipping board");
            skateBoard();
        }
    }

    void OnGUI() {
        GUI.Box(new Rect(100, 10, Screen.width / 5, Screen.height / 10), "press z to board");
    }

    void skateBoard () {
        hasBoard = !hasBoard;
        
        if (hasBoard) {
            animator.SetBool("IsOnBoard", true);
            //  rig.AddRelativeForce(Vector3.down * 10f);
        } else {
            animator.SetBool("IsOnBoard", false);
        }

    }
}