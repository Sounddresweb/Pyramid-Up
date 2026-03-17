using UnityEngine;

public class MummyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 4f;
    public float jumpForce = 11f;

    [Header("Salto variable (sensación pesada)")]
    public float fallMultiplier = 3.5f;      // qué tan rápido cae
    public float lowJumpMultiplier = 2.5f;   // si suelta el botón antes del pico

    [Header("Agacharse")]
    public float crouchScaleY = 0.5f;
    private Vector3 originalScale;
    private bool isCrouching;

    [Header("Detección de suelo")]
    public Transform groundCheck;            // objeto vacío hijo, puesto en los pies
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        // Evita que la momia se voltee al chocar con paredes
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // --- Detección de suelo con OverlapCircle (confiable) ---
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        // --- Movimiento lateral ---
        moveInput = Input.GetAxis("Horizontal");

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reset velocidad Y
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- Salto variable: cae más rápido según el GDD ---
        if (rb.linearVelocity.y < 0)
        {
            // Cayendo: aplicar gravedad extra
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                           * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Subiendo pero soltó el botón: salto corto
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                           * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // --- Agacharse ---
        isCrouching = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        transform.localScale = isCrouching
            ? new Vector3(originalScale.x, crouchScaleY, originalScale.z)
            : originalScale;
    }

    void FixedUpdate()
    {
        // Bloquea movimiento lateral mientras se agacha (opcional, según GDD)
        float currentSpeed = isCrouching ? 0f : speed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    // Visualizar el groundCheck en el editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}