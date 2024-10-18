using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    public float speed = 5f; // Speed modifier
    public Transform target; // Target GameObject to rotate towards 
    public Vector3 rotationOffset; // Offset for the look rotation
    public Material targetMaterial;
    public int numSegments = 20; // Number of segments around the base
    public float height = 5f;    // Height of the cone
    public float radius = 2f;  
    private CharacterController controller; // Character Controller component
    private MeshFilter meshFilter; // MeshFilter component for the cone
    private MeshRenderer meshRenderer;


    void Start()
    {
        controller = GetComponent<CharacterController>(); // Initialize the Character Controller
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = targetMaterial; // Assign the target material to the mesh renderer
        CreateConeMesh();

        transform.position = new Vector3(transform.position.x, transform.position.y, -1); // Set origin Z to -1
        if (target != null)
        {
            target.position = new Vector3(target.position.x, target.position.y, -1); // Set target Z to -1
        }
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
        else
        {
             meshFilter.mesh = null;
        }
    }

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
    }

}
