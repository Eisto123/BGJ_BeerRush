using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public PlayerControl playerControl;
    private Vector2 playerMovement;

    [Header("Movement Config")]
    public float autoMoveSpeed;
    public float leftRightSpeed;

    [Header("Balancing Config")]
    public Slider balanceSlider; // Assign in Inspector
    public float balance = 0f;   // Ranges from -1 to 1
    public float balanceSpeed = 0.2f; // Speed of natural imbalance
    public float playerCorrection = 0.5f; // Player's corrective force
    public float fallThreshold = 1f; // When the player falls
    
    
    private Rigidbody2D rb;


    void Awake()
    {
        playerControl = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();
    }


    void OnEnable()
    {
        playerControl.Enable();
    }

    void OnDisable()
    {
        playerControl.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement = playerControl.Player.Move.ReadValue<Vector2>();

        CheckBalance();
    }

    void FixedUpdate()
    {
        MoveForward();
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector2(playerMovement.x * leftRightSpeed * Time.deltaTime, rb.velocity.y);
    }

    private void MoveForward()
    {
        rb.velocity= Vector2.up * autoMoveSpeed;
    }

    private void CheckBalance()
    {
        // Simulate balance shift toward edges
        if(balance<0){
            balance -= balanceSpeed * Time.deltaTime;
        }
        else if(balance>0){
            balance += balanceSpeed * Time.deltaTime;
        }
        else{
            balance += Random.Range(-1f,1f) * balanceSpeed * Time.deltaTime;
        }
        

        // Player input to counteract the tilt
        if (playerMovement.x < 0)
        {
            balance -= playerCorrection * Time.deltaTime;
        }
        if (playerMovement.x > 0)
        {
            balance += playerCorrection * Time.deltaTime;
        }

        // Clamp balance within range
        balance = Mathf.Clamp(balance, -fallThreshold, fallThreshold);

        if (balanceSlider!=null)
        {
            balanceSlider.value = (balance + fallThreshold) / (2 * fallThreshold); // Normalize to [0,1]
        }

        // Check for fall
        if (Mathf.Abs(balance) >= fallThreshold)
        {
            Debug.Log("Player has fallen!");
            // Handle fall logic (reset, lose condition, etc.)
        }
    }
    
}
