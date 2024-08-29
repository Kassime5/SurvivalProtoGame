using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationPrefab;
    public static NotificationManager instance;
    private List<(Notification, float, int)> activeNotifications = new List<(Notification, float, int)>(); // (Notification, timeToLive, position)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            activeNotifications[i] = (activeNotifications[i].Item1, activeNotifications[i].Item2 - Time.deltaTime, activeNotifications[i].Item3);
            if (activeNotifications[i].Item2 <= 0)
            {
                activeNotifications[i].Item1.DestroyNotification();
            }
        }
    }
    public void CreateNotification(Sprite icon, string name, int amount)
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            if (activeNotifications[i].Item1.notificationName == name)
            {
                activeNotifications[i].Item1.UpdateNotification(icon, amount);
                activeNotifications[i] = (activeNotifications[i].Item1, 3f, activeNotifications[i].Item3);
                return;
            }
        }

        GameObject notification = Instantiate(notificationPrefab, transform);
        notification.GetComponent<Notification>().InitializeNotification(icon, name, amount);

        int freePosition = AdjustNotificationPosition(notification);
        activeNotifications.Add((notification.GetComponent<Notification>(), 3f, freePosition));
    }
    public void PickingUpItemNotification(string itemName, Sprite itemIcon, int amount)
    {
        CreateNotification(itemIcon, itemName, amount);
    }
    private int AdjustNotificationPosition(GameObject newNotification)
    {
        int freePosition = 0;
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            if (activeNotifications[i].Item3 == freePosition)
            {
                freePosition++;
            }
        }

        float yOffset = 85f;
        Vector3 newPosition = new Vector3(0, freePosition * yOffset, 0);
        RectTransform newRectTransform = newNotification.GetComponent<RectTransform>();
        newRectTransform.localPosition = newPosition;


        // if (activeNotifications.Count > 0)
        // {
        //     GameObject lastNotification = activeNotifications[activeNotifications.Count - 1].Item1.gameObject;
        //     RectTransform lastRectTransform = lastNotification.GetComponent<RectTransform>();
        //     newPosition = lastRectTransform.localPosition - new Vector3(0, yOffset, 0);
        // }
        // RectTransform newRectTransform = newNotification.GetComponent<RectTransform>();
        // newRectTransform.localPosition = newPosition;
        return freePosition;
    }
    public void CraftFailedNotification(Sprite icon)
    {
        CreateNotification(icon, "CraftFailed", 0);
    }
    public void RemoveNotification(Notification notification)
    {
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            if (activeNotifications[i].Item1 == notification)
            {
                activeNotifications.RemoveAt(i);
                Destroy(notification.gameObject);
                return;
            }
        }
    }
}
