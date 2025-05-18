using UnityEngine;

public class openLicense : MonoBehaviour
{
    bool isOpen = false;
    public void open()
    {
        isOpen = !isOpen;
        if (isOpen)
            GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        else
            GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -470);
    }

}
