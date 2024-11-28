using UnityEngine;
using System.Collections.Generic;

public class StripControl : MonoBehaviour
{
    public List<GameObject> children = new List<GameObject>();
    private Vector3 previousPosition;

    private void Start()
    {
        // Inicializar a posição anterior com a posição inicial do objeto pai
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Calcular a diferença de posição do objeto pai entre atualizações
        Vector3 deltaPosition = transform.position - previousPosition;

        // Mover cada objeto na lista de filhos de acordo com a diferença de posição
        foreach (GameObject child in children)
        {
            if (child != null)
            {
                child.transform.position += deltaPosition;
            }
        }

        // Atualizar a posição anterior com a posição atual
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fruit")) // Assuming the objects have the tag "fruit"
        {
            // Adicionar o objeto à lista de filhos
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


    // não é necessário.
    private void OnDestroy()
    {
        
        children.Clear();
    }


}
