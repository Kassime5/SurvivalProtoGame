using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Image background;
    public string notificationName;
    private int quantity;
    private NotificationManager notificationManager;

    private void Start()
    {
        notificationManager = NotificationManager.instance;
    }

    public void InitializeNotification(Sprite icon, string name, int amount)
    {
        if (name == "CraftFailed")
        {
            text.text = "Crafting failed!";
            text.color = Color.red;
        }
        else
        {
            text.text = "Picked up " + amount + " " + name;
            text.color = Color.white;
            quantity = amount;
        }
        image.sprite = icon;
        notificationName = name;
    }

    public void UpdateNotification(Sprite icon, int amount)
    {
        if (notificationName == "CraftFailed")
        {
            image.sprite = icon;
            return;
        }
        quantity += amount;
        text.text = "Picked up " + quantity + " " + notificationName;
    }

    public void DestroyNotification()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeTime = 0.25f;
        float fadeSpeed = 1f / fadeTime;

        Color backgroundColor = background.color;
        Color textColor = text.color;
        Color imageColor = image.color;

        while (fadeTime > 0)
        {
            fadeTime -= Time.deltaTime;
            float alpha = fadeTime * fadeSpeed;

            backgroundColor.a = alpha;
            textColor.a = alpha;
            imageColor.a = alpha;

            background.color = backgroundColor;
            text.color = textColor;
            image.color = imageColor;
        }
        notificationManager.RemoveNotification(this);
        yield return null;
        // Destroy(gameObject);
    }
}
