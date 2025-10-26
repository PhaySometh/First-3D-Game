using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy collision detector - Detects when enemy catches the player
/// </summary>
public class EnemyCollisionDetector : MonoBehaviour
{
    private bool hasCollided = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (hasCollided)
            return;

        // Check if it's the player
        if (collision.CompareTag("Player") || collision.gameObject.name.Contains("Player"))
        {
            hasCollided = true;
            Debug.Log("Player caught by " + gameObject.name);

            // Notify GameManager
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerCaught();
            }

            // Disable enemy
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (hasCollided)
            return;

        if (collision.CompareTag("Player") || collision.gameObject.name.Contains("Player"))
        {
            hasCollided = true;
            Debug.Log("Player caught by " + gameObject.name);

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerCaught();
            }

            Destroy(gameObject);
        }
    }
}
