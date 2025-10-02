using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TopoMovement[] topos;           
    public float tiempoEntreTopos = 2f; 
    public float tiempoVisible = 1.5f; 

    void Start()
    {
        StartCoroutine(CicloTopos());
    }

    private System.Collections.IEnumerator CicloTopos()
    {
        while (true)
        {
            Debug.Log("Topo subiendo");

            List<TopoMovement> toposActivos = new List<TopoMovement>();
            foreach (TopoMovement topo in topos)
            {
                if (topo.gameObject.activeInHierarchy)
                    toposActivos.Add(topo);
            }

            if (toposActivos.Count > 0)
            {
                int index = Random.Range(0, toposActivos.Count);
                TopoMovement topoSeleccionado = toposActivos[index];
                topoSeleccionado.Aparecer(tiempoVisible);
            }

            yield return new WaitForSeconds(tiempoEntreTopos);
        }
    }
}
