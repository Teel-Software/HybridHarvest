using System;
using UnityEngine;

public class FitSpriteInScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // вычисляю соотношение сторон
        var aspectRatio = Math.Round((double)Camera.main.scaledPixelWidth / Camera.main.scaledPixelHeight, 1);
        float width;
        float height;

        // перебор наиболее популярных соотношений
        switch (aspectRatio)
        {
            case 1.3:
                width = 418.6723f;
                height = 558.7105f;
                break;
            case 1.6:
                width = 457.4478f;
                height = 510.4752f;
                break;
            case 1.8:
                width = 481.22f;
                height = width;
                break;
            case 2.0:
                width = 521.6415f;
                height = 474.4294f;
                break;
            case 2.1:
                width = 528.6266f;
                height = 441.459f;
                break;
            case 2.2:
                width = 537.8857f;
                height = 433.8546f;
                break;
            default:
                width = 554.407f;
                height = 566.6858f;
                break;
        }

        transform.localScale = new Vector3(width, height, 0);
    }
}
