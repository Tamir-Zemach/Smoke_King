using System;
using UnityEngine;

public class LampHit : MonoBehaviour
{
    private Rigidbody2D lampRb;
    public LayerMask layersToAffect;
    public float force = 5f;

    private void Awake()
    {
        lampRb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other object's layer is inside the mask
        if (((1 << other.gameObject.layer) & layersToAffect) != 0)
        {
            print($"{other.gameObject.name} entered lamp");
            Vector2 dir = (transform.position - other.transform.position).normalized;
            lampRb.AddForce(dir * force, ForceMode2D.Impulse);
            Debug.Log($"Lamp velocity: {lampRb.linearVelocity}");



        }
    }
}
