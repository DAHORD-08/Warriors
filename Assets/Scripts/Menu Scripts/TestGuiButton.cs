using UnityEngine;

public class FlagHover : MonoBehaviour
{
    [Header("Référence au texte à afficher")]
    public GameObject textToShow; // Glisse ici le TextMeshPro enfant

    private void Start()
    {
        if (textToShow != null)
            textToShow.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (textToShow != null)
            textToShow.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (textToShow != null)
            textToShow.SetActive(false);
    }
}