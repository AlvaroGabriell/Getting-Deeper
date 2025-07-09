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
    [Header("Tipo de Nota (Você deve escolher o tipo de nota que deseja)")]
    [Tooltip("1. Echostase\n2. Aracnise\n3. Sarcófago Andorinha\n4. Mariposa\n5. Sombra")]
    [Range(1, 5)]
    public int noteType;

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
