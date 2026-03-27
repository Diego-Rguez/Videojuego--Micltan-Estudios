using UnityEngine;

public class BowController : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    [SerializeField] private GameObject flechaPrefab;
    [SerializeField] private Transform puntoDeDisparo;
    [SerializeField] private float velocidadFlecha = 10f;
    [SerializeField] private int danioFlecha = 10;
    [SerializeField] private float tiempoCargaMaxima = 2f;

    [Header("Trayectoria")]
    [SerializeField] private LineRenderer lineaTrayectoria;
    [SerializeField] private int puntosTrayectoria = 20;
    [SerializeField] private float distanciaTrayectoria = 5f;

    private float tiempoCargaActual = 0f;
    private bool cargando = false;
    private Vector2 direccionDisparo;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        ApuntarConMouse();

        if (Input.GetMouseButtonDown(0))
        {
            IniciarCarga();
        }

        if (Input.GetMouseButton(0) && cargando)
        {
            Cargar();
        }

        if (Input.GetMouseButtonUp(0) && cargando)
        {
            Disparar();
        }
    }

    private void IniciarCarga()
    {
        tiempoCargaActual = 0f;
        cargando = true;
        lineaTrayectoria.enabled = true;
        playerMovement?.BloquearMovimiento();
    }

    private void Disparar()
    {
        float porcentajeCarga = tiempoCargaActual / tiempoCargaMaxima;
        float velocidadFinal = velocidadFlecha * porcentajeCarga;

        if (porcentajeCarga >= 0.1f)
        {
            GameObject flechaObj = Instantiate(flechaPrefab, puntoDeDisparo.position, Quaternion.identity);
            ArrowController flecha = flechaObj.GetComponent<ArrowController>();

            if (flecha != null)
            {
                flecha.Inicializar(direccionDisparo, velocidadFinal, danioFlecha);
            }
        }

        tiempoCargaActual = 0f;
        cargando = false;
        lineaTrayectoria.enabled = false;
        playerMovement?.DesbloquearMovimiento();
    }

    private void MostrarTrayectoria()
    {
        lineaTrayectoria.positionCount = puntosTrayectoria;

        float porcentajeCarga = tiempoCargaActual / tiempoCargaMaxima;
        float velocidadFinal = velocidadFlecha * Mathf.Max(porcentajeCarga, 0.4f);

        // Simula la misma gravedad que usa ArrowController
        float gravedad = 8f;
        Vector2 velocidadSim = direccionDisparo * velocidadFinal;
        Vector3 posicionSim = puntoDeDisparo.position;

        float intervalo = distanciaTrayectoria / puntosTrayectoria;

        for (int i = 0; i < puntosTrayectoria; i++)
        {
            lineaTrayectoria.SetPosition(i, posicionSim);

            // Avanza la simulación paso a paso igual que el Update de la flecha
            velocidadSim.y -= gravedad * intervalo;
            posicionSim += (Vector3)velocidadSim * intervalo;
        }
    }
    private void ApuntarConMouse()
    {
        Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicionMouse.z = 0f;

        direccionDisparo = (posicionMouse - puntoDeDisparo.position).normalized;

        float angulo = Mathf.Atan2(direccionDisparo.y, direccionDisparo.x) * Mathf.Rad2Deg;
        angulo = Mathf.Clamp(angulo, -80f, 80f);

        float anguloRad = angulo * Mathf.Deg2Rad;
        direccionDisparo = new Vector2(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad));

        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    private void Cargar()
    {
        tiempoCargaActual = Mathf.Min(tiempoCargaActual + Time.deltaTime, tiempoCargaMaxima);
        MostrarTrayectoria();
    }
}