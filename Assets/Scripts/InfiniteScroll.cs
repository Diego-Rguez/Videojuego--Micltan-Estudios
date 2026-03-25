using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private Transform otherPiece;
    [SerializeField] private float pieceWidth = 50f;
    [SerializeField] private float resetPositionX = -60f;

    void Update()
    {
        if (transform.position.x >= resetPositionX)
        {
            float newX = otherPiece.position.x - pieceWidth;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
