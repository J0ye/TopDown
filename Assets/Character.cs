using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    public float speed = 5f; // Speed modifier
    public Transform target; // Target GameObject to rotate towards 
    public Vector3 rotationOffset; // Offset for the look rotation
    [Header("Comabt")]
    public GameObject projectilePrefab;
    public string targetTag = "Target"; // Tag to filter objects
    public float detectionRange = 2f;
    public float projectileRotOffset;
    private CharacterController controller; // Character Controller component
    private List<GameObject> objectsInRange = new List<GameObject>(); // List to store objects in range
    
    private LineRenderer lineRenderer;

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Initialize the Character Controller
        /*meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = targetMaterial; // Assign the target material to the mesh renderer
        CreateConeMesh();*/

        transform.position = new Vector3(transform.position.x, transform.position.y, -1); // Set origin Z to -1
        if (target != null)
        {
            target.position = new Vector3(target.position.x, target.position.y, -1); // Set target Z to -1
        }
        lineRenderer = gameObject.GetComponent<LineRenderer>(); // Initialize Line Renderer
        lineRenderer.positionCount = 2; // Set the number of positions
        
    }

    void Update()
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveVertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        // Create a movement vector
        Vector3 move = new Vector3(moveHorizontal, moveVertical, 0);

        // Move the character
        controller.Move(move * speed * Time.deltaTime);

        // Rotate towards the target
        if (target != null) // Check if target is assigned
        {
            Vector3 direction = (target.position - transform.position).normalized; // Calculate direction to target
            if (direction != Vector3.zero) // Ensure direction is not zero
            {
                // Calculate the angle to rotate only on the Z-axis
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset.z; // Get angle in degrees
                Quaternion lookRotation = Quaternion.Euler(0, 0, angle); // Create rotation only on Z-axis
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed); // Smoothly rotate towards target
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PropelProjectile();
        }

        /*
        else
        {
             meshFilter.mesh = null;
        }*/
        DrawLineToNearestTarget();
        CheckForObjectsInRange();
    }

    void PropelProjectile()
    {
        if (objectsInRange.Count > 0) // Check if there are objects in range
        {
            GameObject nearestObject = objectsInRange.OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position)).First(); // Find the nearest object
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity); // Instantiate the projectile at the character's position

            // Calculate direction to the nearest object
            Vector3 direction = (nearestObject.transform.position - transform.position).normalized;

            // Calculate the angle to rotate only on the Z-axis with an offset of 90 degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + projectileRotOffset; // Get angle in degrees with offset
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle); // Set the rotation of the projectile

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>(); // Add Rigidbody2D to the projectile
            rb.velocity = direction * 10f; // Propel the projectile towards the nearest object

            // Ignore collision between the character and the projectile
            //Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Destroy(projectile, 1f); // Destroy the projectile after 3 seconds
        }
    }
    /*
    void CreateConeMesh()
    {
        Mesh mesh = new Mesh();

        // Arrays for vertices and triangles
        Vector3[] vertices = new Vector3[numSegments + 2]; // +2 for the center of the base and the tip
        int[] triangles = new int[numSegments * 3 + numSegments * 3]; // Adjusted for both base and sides

        // Set vertices
        vertices[0] = Vector3.zero; // Center of base at (0,0)

        // Generate the base vertices in the XY plane
        for (int i = 0; i < numSegments; i++)
        {
            float angle = (i / (float)numSegments) * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices[i + 1] = new Vector3(x, y, 0); // Base vertices in the XY plane
        }

        // Set the vertex at the tip of the cone
        vertices[numSegments + 1] = new Vector3(0, height, 0); // Tip of the cone in the Y direction

        // Create triangles for the base
        for (int i = 0; i < numSegments; i++)
        {
            triangles[i * 3] = 0;               // Center of the base
            triangles[i * 3 + 1] = i + 1;       // Current base vertex
            triangles[i * 3 + 2] = (i + 2 <= numSegments) ? i + 2 : 1; // Next base vertex, or wrap around
        }

        // Create triangles for the sides of the cone
        for (int i = 0; i < numSegments; i++)
        {
            triangles[(numSegments + i) * 3] = i + 1;                // Base vertex
            triangles[(numSegments + i) * 3 + 1] = numSegments + 1;  // Tip vertex
            triangles[(numSegments + i) * 3 + 2] = (i + 2 <= numSegments) ? i + 2 : 1; // Next base vertex
        }

        // Apply vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;
    }*/
    void DrawLineToNearestTarget()
    {
        if (objectsInRange.Count > 0) // Check if there are objects in range
        {
            GameObject nearestObject = objectsInRange.OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position)).First(); // Find the nearest object
            lineRenderer.SetPosition(0, transform.position); // Set the start position of the line
            lineRenderer.SetPosition(1, nearestObject.transform.position); // Set the end position of the line
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position); // Set start position
            lineRenderer.SetPosition(1, transform.position); // Set end position to hide the line
        }
    }
    void CheckForObjectsInRange()
    {
        // Clear the list before checking
        objectsInRange.Clear();

        // Find all objects with the specified tag
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in allObjects)
        {
            // Calculate distance to the object
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance <= detectionRange)
            {
                objectsInRange.Add(obj); // Add to the list if within range
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw a circle to visualize the detection range
        Gizmos.color = Color.red; // Set the color for the Gizmo
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Draw the wire sphere
    }
}
