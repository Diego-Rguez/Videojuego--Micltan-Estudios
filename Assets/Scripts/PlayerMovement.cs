using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Transform[] piezas;
    [SerializeField] private Transform boss;

    void Start()
    {
        
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        if (horizontalInput > 0) horizontalInput = 0;

        foreach (Transform pieza in piezas)
        {
            pieza.Translate(Vector2.right * -horizontalInput * moveSpeed * Time.deltaTime);
        }

        if (boss != null)
        {
            boss.Translate(Vector2.right * -horizontalInput * moveSpeed * Time.deltaTime);
        }
    }
}