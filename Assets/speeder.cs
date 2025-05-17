using System.Collections.Generic;
using UnityEngine;

public class speeder : MonoBehaviour
{

    public List<float> speeds;
    int currentIndex = 0;
    public void ChangeSpeed()
    {
        Time.timeScale = speeds[++currentIndex % speeds.Count];
    }

}
