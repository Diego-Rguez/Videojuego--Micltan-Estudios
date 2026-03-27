using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private Transform[] piezas;
    [SerializeField] private Transform boss;

    [Header("Brinco")]
    [SerializeField] private float fuerzaBrinco = 8f;
    [SerializeField] private float gravedad = 20f;
    [SerializeField] private float limiteY = 0f;

    private float velocidadVertical = 0f;
    private bool enElSuelo = true;
    private float posicionYInicial;
    private bool estaDisparando = false;

    void Start()
    {
        posicionYInicial = transform.position.y;
    }

    void Update()
    {
        ManejarMovimientoHorizontal();
        ManejarBrinco();
    }

    private void ManejarMovimientoHorizontal()
    {
        if (estaDisparando) return;

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

    private void ManejarBrinco()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && enElSuelo)
        {
            velocidadVertical = fuerzaBrinco;
            enElSuelo = false;
        }

        if (!enElSuelo)
        {
            velocidadVertical -= gravedad * Time.deltaTime;
            transform.position += Vector3.up * velocidadVertical * Time.deltaTime;

            if (transform.position.y <= posicionYInicial)
            {
                Vector3 pos = transform.position;
                pos.y = posicionYInicial;
                transform.position = pos;

                velocidadVertical = 0f;
                enElSuelo = true;
            }
        }
    }

    public void BloquearMovimiento()
    {
        estaDisparando = true;
    }

    public void DesbloquearMovimiento()
    {
        estaDisparando = false;
    }
}