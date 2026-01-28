using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Audio")]
    public float stepRate = 0.5f;
    private float nextStepTime;
    private Vector2 moveInput;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    public ItemData pickaxe;
    // Animasi
    [SerializeField] public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.wKey.isPressed) moveY = 1f;
        if (Keyboard.current.sKey.isPressed) moveY = -1f;
        if (Keyboard.current.aKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed) moveX = 1f;

        moveInput = new Vector2(moveX, moveY).normalized;

        // Flip character based on horizontal movement
        if (moveX > 0 && !isFacingRight)
            Flip();
        else if (moveX < 0 && isFacingRight)
            Flip();

        // Update walking animation
        animator.SetBool("isWalking", moveInput != Vector2.zero);

                if (moveInput != Vector2.zero && Time.time >= nextStepTime)

        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxFootstep);
            nextStepTime = Time.time + stepRate;
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
