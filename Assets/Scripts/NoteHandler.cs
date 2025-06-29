using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class NoteHandler : MonoBehaviour
{
    [Header("Referências")]
    public GameObject note;
    public GameObject pressECanvas;
    SpriteResolver resolver;
    [Header("Tipo de Nota")]
    [Tooltip("1. Echostase\n2. Aracnise\n3. Sarcófago Andorinha")]
    public int noteType; // 1: Echostase, 2: Aracnise, 3: Sarcófago Andorinha

    // Start is called before the first frame update
    void Start()
    {
        resolver = GetComponent<SpriteResolver>();
    }

    public int GetNoteType()
    {
        return noteType;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            resolver.SetCategoryAndLabel("nota", "nota_cenario_outlined");
            pressECanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            resolver.SetCategoryAndLabel("nota", "nota_cenario");
            pressECanvas.SetActive(false);
        }
    }
}
