using UnityEngine;

public class CatAI : MonoBehaviour
{
    [Header("Configuracion de Huida")]
    public Transform[] waypoints;
    public float baseSpeed = 3f;
    public float maxSpeed = 7f;
    public float detectionRange = 8f;
    public float jumpForce = 7f;

    [Header("Deteccion de suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Referencias")]
    public Transform player;

    // --- NUEVAS VARIABLES DE AUDIO ---
    private AudioSource catAudio;
    private bool hasMeowed = false;
    // ---------------------------------

    private int currentWaypointIndex = 0;
    private bool isEscaping = false;
    private bool reachedEnd = false;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float jumpCooldown = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Buscamos el componente AudioSource que pusiste en el inspector
        catAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (groundCheck == null || player == null ||
            waypoints.Length == 0 || reachedEnd) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        float distanceToPlayer = Vector2.Distance(
            transform.position, player.position);

        // Lógica de detección y maullido
        if (distanceToPlayer < detectionRange)
        {
            // Si es la primera vez que detecta a la momia, ¡maúlla!
            if (!isEscaping && !hasMeowed)
            {
                if (catAudio != null) catAudio.Play();
                hasMeowed = true;
            }
            isEscaping = true;
        }

        if (isEscaping && currentWaypointIndex < waypoints.Length)
            MoveToWaypoint(distanceToPlayer);
    }

    void MoveToWaypoint(float distanceToPlayer)
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 toTarget = target.position - transform.position;

        float proximity = Mathf.InverseLerp(detectionRange, 0f, distanceToPlayer);
        float currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, proximity);

        float dirX = Mathf.Sign(toTarget.x);
        rb.linearVelocity = new Vector2(dirX * currentSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(
            dirX * Mathf.Abs(transform.localScale.x),
            transform.localScale.y, 1f);

        jumpCooldown -= Time.deltaTime;
        if (isGrounded && jumpCooldown <= 0f && toTarget.y > 0.5f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCooldown = 1f;
        }

        if (Mathf.Abs(toTarget.x) < 1f && Mathf.Abs(toTarget.y) < 1.5f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                reachedEnd = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length - 1; i++)
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
    }
}