using UnityEngine;

public class laser : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 5);

        if (transform.position.y > 9f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Laser hit enemy: " + other.name);
            Destroy(other.gameObject);     // Destroy the enemy
            Destroy(this.gameObject);      // Destroy the laser
        }
    }
}
