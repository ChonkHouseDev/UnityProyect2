using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditorInternal;


public class PlayerMovement : MonoBehaviour
{
   [Header("Movement")] 
   private float moveSpeed;
   public float walkSpeed;
   public float sprintSpeed;
   
   [Header("Vivo")] 
   [SerializeField] protected int vidaMax;
   private int _vida;
   
   public float groundDrag;

   [Header("Jumping") ]
   public float jumpForce;
   public float jumpCooldown;
   public float airMultiplier;
   private bool readyToJump;

   [Header("Crouching")] 
   public float crouchSpeed;
   public float crouchYScale;
   private float startYScale;
   
   [Header("Keybinds")] 
   public KeyCode jumpKey = KeyCode.Space;
   public KeyCode sprintKey = KeyCode.LeftShift;
   public KeyCode crouchKey = KeyCode.LeftControl;
   

   [Header("Ground Check")] 
   public float playerHeight;
   public LayerMask whatIsGround;
   private bool grounded;

   [Header("Slope Handling")] 
   public float maxSlopeAngle;
   private RaycastHit slopeHit;
   private bool exitingSlope;

   [Header("UI")] 
   [SerializeField] private Image imagenBarraVida;

   [SerializeField] private TMP_Text cantUIFuego;
   [SerializeField] private TMP_Text cantUIAgua;
   [SerializeField] private TMP_Text cantUITierra;
   
   public Transform orientation;

   private float horizontalInput;
   private float verticalInput;

   private Vector3 moveDirection;
   private Rigidbody rb;

   public MovementState state;

   public enum MovementState
   {
      walking,
      sprinting,
      crouching,
      air
   }
   [SerializeField]private Transform personaje;
   private Animator animator;

   public PlayerCam playercam;

   [SerializeField]private Transform checkPoint;
   
   #region Items
   public int _cantidadFuego;

   private int CantidadFuego
   {
      get => _cantidadFuego;
      set
      {
         _cantidadFuego = value;
         cantUIFuego.text =value.ToString();
      }
   }
   
   public int _cantidadAgua;

   private int CantidadAgua
   {
      get => _cantidadAgua;
      set
      {
         _cantidadAgua = value;
         cantUIAgua.text =value.ToString();
      }
   }
   
   public int _cantidadTierra;

   private int CantidadTierra
   {
      get => _cantidadTierra;
      set
      {
         _cantidadTierra = value;
         cantUITierra.text =value.ToString();
      }
   }
   #endregion Items
   
   
   
   private void Start()
   {
      rb = GetComponent<Rigidbody>();
      rb.freezeRotation = true;
      
      readyToJump = true;

      startYScale = transform.localScale.y;
   }

   private void Update()
   {
     //ground check
      grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
      
      MyInput();
      SpeedControl();
      StateHandler();
      Animaciones();
      
      //handle drag
      if (grounded)
      {
         rb.drag = groundDrag;   
      }
      else
      {
         rb.drag = 0;
      }
      
      //print(Convert.ToString(Vida) );
   }

   private void FixedUpdate()
   {
      MovePlayer();
   }

   private void Awake()
   {

      animator = personaje.GetChild(1).GetComponent<Animator>();
      
      Vida = vidaMax;
      CantidadFuego = 0;
      CantidadAgua = 0;
      CantidadTierra = 0;
     
   }

   private void MyInput()
   {
      horizontalInput = Input.GetAxisRaw("Horizontal");
      verticalInput = Input.GetAxisRaw("Vertical");
      
      //when to Jump
      if (Input.GetKey(jumpKey) && readyToJump && grounded)
      {
         readyToJump = false;
         Jump();
         Invoke(nameof(ResetJump), jumpCooldown);
      }

         //start crouch
      if (Input.GetKeyDown(crouchKey))
      {
         transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
         rb.AddForce(Vector3.down*5f,ForceMode.Impulse);
      }

         //stop crouch  
      if (Input.GetKeyUp(crouchKey))
      {
         transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
      }
      
      
      
   }

   private void StateHandler()
   {
      //mode - crouching
      if (Input.GetKey(crouchKey))
      {
         state = MovementState.crouching;
         moveSpeed = crouchSpeed;
      }
      
      // Mode - sprinting
      if (grounded && Input.GetKey(sprintKey))
      {
         state = MovementState.sprinting;
         moveSpeed = sprintSpeed;
      }
      
      //Mode - walking
      else if (grounded)
      {
         state = MovementState.walking;
         moveSpeed = walkSpeed;
      }
      
      //Mode - air
      else
      {
         state = MovementState.air;
      }
      
   }

   private void Animaciones()
   {
      switch (state)
      {
         case MovementState.sprinting:
            animator.SetBool("run",true);
            print("corriendo");
            break;
         
         case MovementState.walking:
            animator.SetBool("run",false);
            print("caminando");
            break;
         
         default:
            //default   
            break;
      }
   }

   private void MovePlayer()
   {
      //calculate movement direction
      moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
      
      //on slope
      if (OnSlope() && !exitingSlope)
      {
         rb.AddForce(GetSLopeMoveDirection()*moveSpeed*20f,ForceMode.Force);

         if (rb.velocity.y > 0)
         {
            rb.AddForce(Vector3.down*80f, ForceMode.Force);
         }
      }
      
      //on ground
      if(grounded)
         rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
      
      //in air
      else if(!grounded)
         rb.AddForce(moveDirection.normalized*moveSpeed*10f*airMultiplier,ForceMode.Force);
      
      //turn gravity off while on slope
      rb.useGravity = !OnSlope();
   }

   private void SpeedControl()
   {
      //limitin speed on slope
      if (OnSlope() && !exitingSlope)
      {
         if (rb.velocity.magnitude > moveSpeed)
         {
            rb.velocity = rb.velocity.normalized * moveSpeed;
         }
      }
      else
      {
         Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
         
         // limit velocity if needed
         if (flatVel.magnitude > moveSpeed)
         {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
         }
         //print("Speed: "+flatVel.magnitude);
      }
      
   }

   private void Jump()
   {
      exitingSlope = true;
      animator.SetTrigger("salto");
      // reset y velocity
      rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);
   }

   private void ResetJump()
   {
      readyToJump = true;

      exitingSlope = false;
   }

   private void DanoEnemigo()
   {
      
      Vida = Vida - 5;
   }

   private bool OnSlope()
   {
      if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
      {
         float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
         return angle < maxSlopeAngle && angle != 0;
      }

      return false;
   }

   private Vector3 GetSLopeMoveDirection()
   {
      return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
   }

   public void OnTriggerEnter(Collider other)
   {
      switch (other.tag)
      {
         case "PickupFire":
            CantidadFuego++;
            Destroy(other.gameObject);
            print("pickup fuego");
            break;
         
         case "PickupAgua":
            CantidadAgua++;
            Destroy(other.gameObject);
            print("pickup agua");
            break;
         
         case "PickupTierra":
            CantidadTierra++;
            Destroy(other.gameObject);
            print("pickup tierra");
            break;
         
         case "Enemigo":
            print("Colisiono "+other.name);
            DanoEnemigo();
            playercam.SetVignetteActive();
            break;
         
         case "Recover":
            Recover(10);
            break;
         
         case "FullRecover":
            Recover(1000);
            break;
         
         case "FINAL":
            GameManager.Win();
            break;
      }
   }

   private void Recover(int recuperar)
   {
      Vida += recuperar;
   }

   public IEnumerator Morir()
   {
      enabled = false;
      yield return new WaitForSeconds(3f);
      //playercam.SetVignetteMuerte();
      FinalizarMuerte();
   }

   private void FinalizarMuerte()
   {
      transform.position = checkPoint.position;
      enabled = true;
      Vida = vidaMax;
      //playercam.RSetVignette();
   }
   
   public int Vida
   {
      get
      {
         return _vida;
      }

      set
      {
         if (value <= 0)
         {
            StartCoroutine(Morir());
            return;
         }
         else if (value >= vidaMax)
         {
            _vida = vidaMax;
         }
         else
         {
            if (value < _vida)
            {
               //StartCoroutine(CrColorDano());
            }

            _vida = value;
         }

         imagenBarraVida.fillAmount = (float)_vida / vidaMax;

      }
   }
}
