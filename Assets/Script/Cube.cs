using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool canMove;
    public float moveSpeed;
    private bool hasTriggeredEnd = false;

    private void Start()
    {
        moveSpeed = GameManager.Instance.CubeMoveSpeed;
    }

    private void Update()
    {
        if (canMove)
        {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
        }

        
        if (transform.position.y < -5f && !hasTriggeredEnd && !GameManager.Instance.IsGameOver)
        {
            hasTriggeredEnd = true;
            GameManager.Instance.EndGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Collider") || other.CompareTag("Plane")) && !hasTriggeredEnd && !GameManager.Instance.IsGameOver)
        {
            hasTriggeredEnd = true;
            GameManager.Instance.EndGame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            GameManager.Instance.CollectedCubeCount++;
            GameManager.Instance.SpawnNewCube();

            if (GameManager.Instance.CollectedCubeCount == 1)
            {
                collision.gameObject.tag = "Untagged";
            }
        }
    }
}