using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Station : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public Stop stop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int minutes = ((int)(stop.timeUntilNextStop / 60));
        int seconds = ((int)(stop.timeUntilNextStop % 60));
        int hundreths = (int)((stop.timeUntilNextStop - Mathf.Floor(stop.timeUntilNextStop)) * 100f);
        text1.text = text2.text = text3.text = stop.timeUntilNextStop > 0 && stop.timeUntilNextStop + stop.stopTime < stop.totalTimeForPath ? string.Format("<mspace=mspace=2.2>{0,2}</mspace>:<mspace=mspace=2.2>{1,2}</mspace>", (minutes > 0 ? minutes : seconds).ToString("D2"), (minutes > 0 ? seconds : hundreths).ToString("D2")) :
            (int)(Time.time * 2f) % 2 == 0 ? "ARR" : "";
    }
}
