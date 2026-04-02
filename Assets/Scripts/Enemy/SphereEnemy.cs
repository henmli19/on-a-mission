using System.Collections;
using UnityEngine;

public class SphereEnemy : Enemy
{
    [Header("Sphere Specifics")]
    public float grabSpeed = 4f;
    public float pullUpForce = 2.5f;
    public int escapePresses = 12;

    private bool isGrabbing = false;
    private Vector2 roamTarget;

    protected override void Start()
    {
        base.Start();
        roamTarget = transform.position;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        if (isDead || isGrabbing) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
            ChasePlayer();
        else
            Patrol();

        float hover = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position += new Vector3(0, hover * Time.deltaTime, 0);
    }

    protected override void ChasePlayer()
    {
        Vector3 target = new Vector3(player.position.x, player.position.y + 0.5f, transform.position.z);
        transform.position = Vector2.MoveTowards(transform.position, target, grabSpeed * Time.deltaTime);
    }

    protected override void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, roamTarget, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, roamTarget) < 0.5f)
            roamTarget = startPosition + (Random.insideUnitCircle * patrolRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isGrabbing)
        {
            Player_Scripts.PlayerMovement pm = other.GetComponent<Player_Scripts.PlayerMovement>();
            if (pm != null && !pm.isShielded)
            {
                StartCoroutine(GrabRoutine(pm));
            }
        }
    }

    private IEnumerator GrabRoutine(Player_Scripts.PlayerMovement pm)
    {
        isGrabbing = true;
        pm.DisableControls();
        pm.StartGrabbingMinigame(escapePresses, this.gameObject);

        // Shield check before dealing grab damage
        if (!pm.isShielded)
        {
            BatteryHealthUI health = pm.GetComponent<BatteryHealthUI>();
            if (health != null) health.TakeDamage(1);
        }

        while (isGrabbing)
        {
            transform.position += Vector3.up * pullUpForce * Time.deltaTime;
            pm.transform.position = transform.position + new Vector3(0, -0.7f, 0);
            yield return null;
        }
    }

    public void ReleasePlayer(Player_Scripts.PlayerMovement pm)
    {
        isGrabbing = false;
        pm.StopGrabbingMinigame();
        pm.EnableControls();

        startPosition = transform.position + Vector3.up * 3f;
        roamTarget = startPosition;
    }

    protected override void Attack() { }
}