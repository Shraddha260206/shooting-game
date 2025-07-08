using UnityEngine;

public class Marshmallow : MonoBehaviour
{
    [SerializeField] private GameObject damagedPrefab;
    private bool isDamaged = false;

    public void SetDamagedPrefab(GameObject prefab)
    {
        damagedPrefab = prefab;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Marshmallow {gameObject.name} was hit by {other.gameObject.name}");
        
        if (other.CompareTag("laser"))
        {
            Debug.Log($"Laser confirmed! isDamaged: {isDamaged}, damagedPrefab: {damagedPrefab?.name}");
            
            Destroy(other.gameObject);

            if (!isDamaged && damagedPrefab != null)
            {
                // Get current velocity before destroying
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                Vector2 currentVelocity = rb != null ? rb.linearVelocity : Vector2.zero;
                
                // First hit: spawn damaged version
                GameObject damaged = Instantiate(damagedPrefab, transform.position, Quaternion.identity);
                
                // Apply the same velocity to continue movement
                Rigidbody2D damagedRb = damaged.GetComponent<Rigidbody2D>();
                if (damagedRb != null)
                {
                    damagedRb.linearVelocity = currentVelocity;
                }
                
                isDamaged = true;
                Destroy(gameObject); // Destroy the full version
            }
            else
            {
                // Second hit: already damaged, destroy
                Destroy(gameObject);
            }
        }
    }
}
