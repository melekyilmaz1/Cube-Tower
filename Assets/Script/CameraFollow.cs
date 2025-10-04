using UnityEngine; 
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private float followSmoothness = 0.02f;
    private float offsetY = 2.5f;
    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 newPosition = new Vector3(
            transform.position.x,
            target.position.y + offsetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, newPosition, followSmoothness);
    }
}