using UnityEngine;
using System.Collections.Generic;

public class StripControl : MonoBehaviour
{
    public List<GameObject> children = new List<GameObject>();
    private Vector3 previousPosition;

    private void Start()
    {
        // Inicializar a posi��o anterior com a posi��o inicial do objeto pai
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Calcular a diferen�a de posi��o do objeto pai entre atualiza��es
        Vector3 deltaPosition = transform.position - previousPosition;

        // Mover cada objeto na lista de filhos de acordo com a diferen�a de posi��o
        foreach (GameObject child in children)
        {
            if (child != null)
            {
                child.transform.position += deltaPosition;
            }
        }

        // Atualizar a posi��o anterior com a posi��o atual
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fruit")) // Assuming the objects have the tag "fruit"
        {
            // Adicionar o objeto � lista de filhos
            children.Add(collision.gameObject);

            // Opcionalmente, pode-se adicionar o objeto como filho na hierarquia
            //collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("fruit")) // Reset parent when object leaves the strip
        {
            // Remover o objeto da lista de filhos
            children.Remove(collision.gameObject);

            // Opcionalmente, pode-se remover o objeto como filho na hierarquia
            //collision.gameObject.transform.SetParent(null);
        }
    }


    // n�o � necess�rio.
    private void OnDestroy()
    {
        
        children.Clear();
    }


}
