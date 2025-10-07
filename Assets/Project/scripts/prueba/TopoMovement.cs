using UnityEngine;
using System.Collections;

public class TopoMovement : MonoBehaviour
{

    public Transform posicionAbajo;
    public Transform posicionArriba;
    public float velocidad = 3f;
    public float tiempoCongelado = 4f; 
    private Vector3 objetivo;
    private bool enMovimiento = false;
    private Coroutine movimientoActual;

    void Start()
    {
        transform.position = posicionAbajo.position;
    }

    public void Aparecer(float tiempoVisible)
    {
        if (!enMovimiento)
            movimientoActual = StartCoroutine(SubirYBajar(tiempoVisible));
    }

    private IEnumerator SubirYBajar(float tiempoVisible)
    {
        enMovimiento = true;

        objetivo = posicionArriba.position;
        while (Vector3.Distance(transform.position, objetivo) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(tiempoVisible);

        objetivo = posicionAbajo.position;
        while (Vector3.Distance(transform.position, objetivo) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
            yield return null;
        }

        enMovimiento = false;
    }

    void OnMouseDown()
    {
        if (enMovimiento)
        {
            if (movimientoActual != null)
                StopCoroutine(movimientoActual);

            StartCoroutine(CongelarYDesaparecer());
        }
    }

//    void Update()
//    {
//#if UNITY_EDITOR || UNITY_STANDALONE
//        if (Input.GetMouseButtonDown(0))
//        {
//            DetectarInteraccion(Input.mousePosition);
//        }
//#elif UNITY_IOS || UNITY_ANDROID
//    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
//    {
//        DetectarInteraccion(Input.GetTouch(0).position);
//    }
//#endif
//    }

//    void DetectarInteraccion(Vector3 posicion)
//    {
//        Ray ray = Camera.main.ScreenPointToRay(posicion);
//        if (Physics.Raycast(ray, out RaycastHit hit))
//        {
//            if (hit.transform == transform && enMovimiento)
//            {
//                if (movimientoActual != null)
//                    StopCoroutine(movimientoActual);

//                StartCoroutine(CongelarYDesaparecer());
//            }
//        }
//    }

    private IEnumerator CongelarYDesaparecer()
    {
        yield return new WaitForSeconds(tiempoCongelado);

        objetivo = posicionAbajo.position;
        while (Vector3.Distance(transform.position, objetivo) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
            yield return null;
        }

        enMovimiento = false;

        gameObject.SetActive(false);
    }
}
