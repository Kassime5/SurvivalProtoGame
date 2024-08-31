using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class CheckCollisionFloor : MonoBehaviour
{
    [SerializeField] private GameObject planks;
    private bool isQuitting = false;
    // private Renderer treeRenderer;
    // private float fadeDuration = 2.0f; // Duration of the fade-out effect

    private void Start()
    {
        Application.quitting += HandleApplicationQuitting;
    }

    private void OnDestroy()
    {
        if (!isQuitting)
        {
            // GameObject plank1 = Instantiate(planks, transform.position, Quaternion.identity);
            // GameObject plank2 = Instantiate(planks, transform.position + new Vector3(1, 1), Quaternion.identity);
        }
    }

    private void HandleApplicationQuitting()
    {
        isQuitting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            GameObject parent = transform.parent.gameObject;
            while (parent.transform.parent != null)
            {
                parent = parent.transform.parent.gameObject;
                if (parent.tag == "Tree")
                {
                    Destroy(parent);
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // private IEnumerator FadeOutAndDestroy()
    // {
    //     float startAlpha = treeRenderer.material.color.a;
    //     float rate = 1.0f / fadeDuration;
    //     float progress = 0.0f;

    //     while (progress < 1.0f)
    //     {
    //         Color tmpColor = treeRenderer.material.color;
    //         treeRenderer.material.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
    //         progress += rate * Time.deltaTime;
    //         yield return null;
    //     }

    //     // Ensure the final alpha is set to 0
    //     Color finalColor = treeRenderer.material.color;
    //     treeRenderer.material.color = new Color(finalColor.r, finalColor.g, finalColor.b, 0);

    //     // Destroy the parent object after the fade-out effect
    //     Destroy(transform.parent.gameObject);
    // }
}