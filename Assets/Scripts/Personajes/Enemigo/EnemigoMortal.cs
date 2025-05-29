using UnityEngine;

/// <summary>
/// Componente para enemigos que pueden morir (con animación, físicas, pooling, etc.).
/// </summary>
[RequireComponent(typeof(SaludSistemaControlador))]
public class EnemigoMortal : EntidadNoJugador
{
    [Header("Muerte de enemigo")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerMuerte = "Muerte";
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool usarFisicaAlMorir = true;
    [SerializeField] private float fuerzaCaida = 10f;

    protected override void OnMuerte()
    {
        // Animación de muerte
        if (animator != null && !string.IsNullOrEmpty(triggerMuerte))
            animator.SetTrigger(triggerMuerte);
        // Físicas (caída, impulso, etc.)
        if (usarFisicaAlMorir && rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * fuerzaCaida, ForceMode.Impulse);
        }
        // Pooling o destrucción (con delay si hay animación)
        Invoke(nameof(FinalizarMuerte), 1.5f);
    }

    private void FinalizarMuerte()
    {
        base.OnMuerte();
    }
}
