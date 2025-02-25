using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
    public Slider balanceSlider;
    public float balance = 0f;   // Ranges from -1 to 1
    public float balanceSpeed = 0.2f; // Speed of natural imbalance
    public float playerCorrection = 0.5f; // Player's corrective force
    public float fallThreshold = 1f; // When the player falls
    
    [Header("Dash Config")]
    public float dashDistance = 2f; // Fixed dash distance
    public float dashCooldown = 2f; // Cooldown of 2 seconds
    public float dashTime = 0.2f; // Duration of dash
    public float dashBalanceReset = 0.5f; // Speed of dash
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Lose Events")]
    public UnityEvent onDeath;
    public bool isDead = false;

    [Header("Time Scaling")]
    public float timeGrowthRate = 0.01f; // Controls how fast time speeds up
    public float maxTimeScale = 3f; // Prevents the game from speeding up infinitely

    private Rigidbody2D rb;
    public Animator playerAnim;
    public BeerHolder beerHolder;
    public GameObject dashIndicator;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public IEnumerator PlayFootStep(){
        if(!playerAudioSource.isPlaying){

            yield return new WaitForSeconds(0.5f);
            playerAudioSource.Play();
        }
    }

    void Awake()
    {
        playerControl = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();
        playerAudioSource = GetComponent<AudioSource>();
        //AudioManager.Instance.EnableAudioSource();
        StartCoroutine(PlayFootStep());
        //StartCoroutine(AudioManager.Instance.PlayBGM(0,0.5f,true));
    }


    void OnEnable()
    {
        playerControl.Enable();
    }

    void OnDisable()
    {
        playerControl.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            playerMovement = playerControl.Player.Move.ReadValue<Vector2>();
            CheckBalance();
            Time.timeScale = Mathf.Min(1f + (Time.timeSinceLevelLoad * timeGrowthRate), maxTimeScale);
        }
        else
        {
            Time.timeScale = 1f;
            playerMovement = Vector2.zero;
        }
        
        if (canDash && (playerMovement.x != 0) && playerControl.Player.Dash.triggered)
        {
            StartCoroutine(Dash(playerMovement.x));
        }
    }

    void FixedUpdate()
    {
        if (!isDashing&&!isDead)
        {
            MoveForward();
            Move();
        }
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
            balance += UnityEngine.Random.Range(-1f,1f) * balanceSpeed * Time.deltaTime;
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

            FallOff();

        }
    }

    private IEnumerator Dash(float direction)
    {
        // Audio
        playerAudioSource.Stop();
        AudioManager.Instance.PlaySFX(1);
        dashIndicator.SetActive(true);
        
        canDash = false;
        isDashing = true;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + new Vector2(direction * dashDistance, 0);
        float elapsedTime = 0f;
        playerAnim.SetTrigger("Dash");
        float balanceAfterDash = playerMovement.x>0 ? balance+dashBalanceReset : balance-dashBalanceReset;
        while (elapsedTime < dashTime)
        {
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsedTime / dashTime));
            balance = Mathf.Lerp(balance, balanceAfterDash, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        rb.MovePosition(targetPos); // Ensure final position is accurate

        isDashing = false;
        
        // Audio
        StartCoroutine(PlayFootStep());

        yield return new WaitForSeconds(dashCooldown);
        StartCoroutine(FadeOutIndicator());
        yield return new WaitForSeconds(1f);
        canDash = true;
    }

    private IEnumerator FadeOutIndicator()
    {
        float elapsedTime = 0f;
        Color startColor = dashIndicator.GetComponent<Image>().color;

        while (elapsedTime < 1f)
        {
            dashIndicator.GetComponent<Image>().color = Color.Lerp(dashIndicator.GetComponent<Image>().color, new Color(1,1,1,0), elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }
        
        dashIndicator.SetActive(false);
        dashIndicator.GetComponent<Image>().color = startColor;
    }

    public void FallOff()
    {
        isDead = true;
        
        playerAnim.SetTrigger("FallOff");

        //Audio break
        playerAudioSource.Stop();
        //AudioManager.Instance.PlaySFX(4);
        StartCoroutine(AudioManager.Instance.PlaySFX(4, 0.75f));

        StartCoroutine(FallOffRoutine());
        Debug.Log("Player has fallen!");
    }
    public void DieByBullet()
    {
        isDead = true;
        playerAnim.SetTrigger("Die");

        //Audio, cry
        playerAudioSource.Stop();
        AudioManager.Instance.PlayEnviroment(3);

        StartCoroutine(FallOffRoutine());
    }

    private IEnumerator FallOffRoutine()
    {
        rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        float deathTime = 1.5f;
        float elapsedTime = 0f;
        Vector2 initialVelocity = rb.velocity;
        while (elapsedTime < deathTime)
        {
            rb.velocity = Vector2.Lerp(initialVelocity, Vector2.zero, elapsedTime / deathTime);
            elapsedTime += Time.deltaTime;
            if (elapsedTime / deathTime> 0.5f)
            {
                onDeath.Invoke();
            }
            yield return null; // Wait for next frame
        }
        rb.velocity = Vector2.zero;

        //UI pannel
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            FallOff();
        }
        if (collision.CompareTag("Bullet"))
        {
            DieByBullet();
        }
    }
    
    public void ReduceBeer(int beerNum)
    {
        StartCoroutine(ReducingBeer(beerNum));
    }
    private IEnumerator ReducingBeer(int beerNum)
    {
        for (int i = 0; i < beerNum; i++)
        {
            beerHolder.RemoveBeer();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void AddBeer(int beerNum)
    {
        //Audio
        // if (!isDead)
        //     AudioManager.Instance.PlaySFX(2);
        
        StartCoroutine(AddingBeer(beerNum));
    }

    private IEnumerator AddingBeer(int beerNum)
    {
        if (!beerHolder.beerIsFull && !isDead)
        {
            //Audio
            AudioManager.Instance.PlaySFX(2);
        }
        for (int i = 0; i < beerNum; i++)
        {
            beerHolder.AddBeer();
            yield return new WaitForSeconds(0.2f);
        }
    }

}
