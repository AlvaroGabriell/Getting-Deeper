/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivesController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteComOutline;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = spriteComOutline;
            other.GetComponent<PlayerInteracao>().MostrarDica(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = spriteNormal;
            other.GetComponent<PlayerInteracao>().EsconderDica(this);
        }
    }
}
*/

// Ainda não implementado.