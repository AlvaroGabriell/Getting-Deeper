using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SombraController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public Transform eyePosition;
    public Animator animator;
    public LayerMask coverLayer;
    public LayerMask playerLayer;
    BoxCollider2D detectionZone;

    [Header("Valores")]
    [Tooltip("Tempo mínimo que o olho fica fechado antes de abrir")]
    public float minInterval = 1f;
    [Tooltip("Tempo máximo que o olho fica fechado antes de abrir")]
    public float maxInterval = 5f;
    [Tooltip("Tempo mínimo que o olho fica aberto antes de fechar")]
    public float minOpenTime = 2f;
    [Tooltip("Tempo máximo que o olho fica aberto antes de fechar")]
    public float maxOpenTime = 5f;
    public float warningDuration = 0.8f;  // tempo de shake antes de abrir
    [Tooltip("Distância pro jogador do olho no qual vai começar a tocar o SFX")]
    public float triggerDistance = 6f;

    [Header("Shake")]
    public float shakeMagnitude = 0.05f;
    public int shakeVibrato = 6;

    private Vector3 eyeInitialPos;
    private bool isEyeOpen = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        detectionZone = GetComponent<BoxCollider2D>();
        eyeInitialPos = eyePosition.localPosition;
        detectionZone.enabled = false;
        StartCoroutine(StateLoop());
    }

    IEnumerator StateLoop()
    {
        while (true)
        {
            // 1) FECHADO
            isEyeOpen = false;
            detectionZone.enabled = false;
            animator.ResetTrigger("OpenEye");
            animator.SetTrigger("CloseEye");
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            // 2) AVISO: treme antes de abrir
            yield return StartCoroutine(ShakeWarning());

            // 3) ABRE e detecta
            animator.ResetTrigger("CloseEye");
            animator.SetTrigger("OpenEye");
            PlayOpenEye();
            yield return new WaitForSeconds(0.2f); // espera abrir um pouquinho

            isEyeOpen = true;
            detectionZone.enabled = true;

            // mantém aberto um tempinho pra dar tempo pro jogador passar
            yield return new WaitForSeconds(Random.Range(minOpenTime, maxOpenTime));
            yield return StartCoroutine(ShakeWarning());
        }
    }

    IEnumerator ShakeWarning()
    {
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);
            eyePosition.localPosition = eyeInitialPos + new Vector3(x, y, 0f);

            elapsed += warningDuration / shakeVibrato;
            yield return new WaitForSeconds(warningDuration / shakeVibrato);
        }
        eyePosition.localPosition = eyeInitialPos;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!isEyeOpen) return;
        if (!collision.CompareTag("Player")) return;

        // Se o olho está aberto e o jogador está na zona de detecção,
        // verifica se ele está exposto usando raycasts em cada parte do corpo do jogador por garantia.
        var playerCollider = collision.GetComponent<PolygonCollider2D>();
        if (playerCollider != null)
        {
            // Monta lista de pontos: center + cada vértice local
            Vector3 center = playerCollider.bounds.center;
            var points = new List<Vector3> { center };
            for (int p = 0; p < playerCollider.pathCount; p++)
            {
                Vector2[] path = playerCollider.GetPath(p);
                foreach (var point in path)
                    points.Add(collision.transform.TransformPoint(point));
            }

            foreach (var target in points)
            {
                Vector2 dir = (target - eyePosition.position).normalized;
                float dist = Vector2.Distance(eyePosition.position, target);
                // LayerMask que considera jogador + coberturas
                int mask = coverLayer | playerLayer;
                RaycastHit2D hit = Physics2D.Raycast(eyePosition.position, dir, dist, mask);
                if (hit.collider == null) continue;
                if (hit.collider.CompareTag("Player"))
                {
                    // primeiro hit foi o jogador -> exposto
                    FindObjectOfType<UIHandler>().chamarGameOver();
                    return;
                }
                // se hit for cobertura, esse ponto está protegido
            }
            // nenhum ponto exposto -> totalmente coberto
            return;
        }
    }

    public void PlayOpenEye()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= triggerDistance)
        {
            SFXManager.Instance.PlaySFX("SombraOpenEye");
        }
    }
}