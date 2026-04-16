using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instancia;

    [Header("Bolas de fuego")]
    [SerializeField] private BossFireball prefabFireball;
    [SerializeField] private int tamanoInicialFireball = 5;
    [SerializeField] private int maximoFireball = 10;

    [Header("Flechas")]
    [SerializeField] private ArrowController prefabFlecha;
    [SerializeField] private int tamanoInicialFlechas = 5;
    [SerializeField] private int maximoFlechas = 20;

    private ObjectPool<BossFireball> poolFireball;
    private ObjectPool<ArrowController> poolFlechas;

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        poolFireball = new ObjectPool<BossFireball>(
            createFunc: () =>
            {
                BossFireball fb = Instantiate(prefabFireball);
                fb.gameObject.SetActive(false);
                fb.SetPool(poolFireball);
                return fb;
            },
            actionOnGet: fb => { },   // la activación se hace en ObtenerFireball, ya posicionado
            actionOnRelease: fb => fb.gameObject.SetActive(false),
            actionOnDestroy: fb => Destroy(fb.gameObject),
            defaultCapacity: tamanoInicialFireball,
            maxSize: maximoFireball
        );

        poolFlechas = new ObjectPool<ArrowController>(
            createFunc: () =>
            {
                ArrowController flecha = Instantiate(prefabFlecha);
                flecha.gameObject.SetActive(false);
                flecha.SetPool(poolFlechas);
                return flecha;
            },
            actionOnGet: flecha => { }, // la activación se hace en ObtenerFlecha, ya posicionada
            actionOnRelease: flecha => flecha.gameObject.SetActive(false),
            actionOnDestroy: flecha => Destroy(flecha.gameObject),
            defaultCapacity: tamanoInicialFlechas,
            maxSize: maximoFlechas
        );

        // Pre-crea los objetos al inicio
        PreCalentar(poolFireball, tamanoInicialFireball);
        PreCalentar(poolFlechas, tamanoInicialFlechas);
    }

    private void PreCalentar<T>(ObjectPool<T> pool, int cantidad) where T : class
    {
        var lista = new System.Collections.Generic.List<T>(cantidad);
        for (int i = 0; i < cantidad; i++)
            lista.Add(pool.Get());
        foreach (var obj in lista)
            pool.Release(obj);
    }

    public BossFireball ObtenerFireball(Vector3 posicion)
    {
        BossFireball fb = poolFireball.Get();
        fb.transform.position = posicion;       // posiciona primero
        fb.transform.rotation = Quaternion.identity;
        fb.gameObject.SetActive(true);          // activa después
        return fb;
    }

    public ArrowController ObtenerFlecha(Vector3 posicion, Quaternion rotacion)
    {
        ArrowController flecha = poolFlechas.Get();
        flecha.transform.position = posicion;   // posiciona primero
        flecha.transform.rotation = rotacion;
        flecha.gameObject.SetActive(true);      // activa después
        return flecha;
    }
}
