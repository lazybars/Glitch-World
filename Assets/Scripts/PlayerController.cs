using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnDistance = 0.1f;

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

    CycleObjects cycler; 

    Animator animator;
    Transform cameraT;
    CharacterController controller;
    Rigidbody rig;

    void Start() {
        if (!objectToSpawn) {
            objectToSpawn = Resources.Load("ESphere") as GameObject;
        }
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        rig = GetComponent<Rigidbody>();
        StartCoroutine(waitAndRemoveGui(2f));
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

        keyActions();
    }

    void keyActions() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            print("equipping board");
            skateBoard();
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            print("spawning object");
            spawnObjectInFront();
        }

        if (Input.GetKey(KeyCode.C)) {
            print("doing action");
            animator.SetTrigger("IsHumping");
        }
    }

    bool guiActive = true;
    void OnGUI() {
        if (guiActive) {
            GUI.Box(new Rect(((Screen.width / 2) - 300f), Screen.height - 100, 300, 100), "press z to board | x to spawn | c to...do that");
        }
    }

    IEnumerator waitAndRemoveGui(float time) {
        yield return new WaitForSeconds(time);
        guiActive = false;
        
    }

    void spawnObjectInFront() {
        Vector3 playerPos = transform.position;
        Vector3 playerDirection = transform.forward;
        Quaternion playerRotation = transform.rotation;

        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;

        Instantiate(objectToSpawn, spawnPos, playerRotation);
    }

    void skateBoard () {
        hasBoard = !hasBoard;        
        if (hasBoard) {
            animator.SetBool("IsOnBoard", true);
        } else {
            animator.SetBool("IsOnBoard", false);
        }

    }
}