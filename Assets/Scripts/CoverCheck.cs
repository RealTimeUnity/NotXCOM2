using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCheck : MonoBehaviour
{
    public Camera characterCamera;
    public Camera worldCamera;

    public static CoverCheck singleton;

    public void Start()
    {
        singleton = this;
        //getShoot(null, null);
    }

    public void setLayer(int num,Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform temp = t.GetChild(i);
            temp.gameObject.layer = num;
            setLayer(num, temp);
        }
    }

    public int getShoot(GameObject target, GameObject src)
    {
        
        characterCamera.transform.parent.position=src.transform.position;
        float halfTheta = Mathf.Atan(4 / (src.transform.position - target.transform.position).magnitude);
        halfTheta = Mathf.Rad2Deg * halfTheta;
        characterCamera.fieldOfView = halfTheta;
        worldCamera.fieldOfView = halfTheta;
        int beforeLayer = target.layer;
        setLayer(8, target.transform);
        characterCamera.transform.LookAt(target.transform);
        worldCamera.transform.LookAt(target.transform);
        characterCamera.Render();
        Texture2D charCamTex = new Texture2D(256, 256);
        RenderTexture.active = characterCamera.activeTexture;
        charCamTex.ReadPixels(new Rect(0, 0, charCamTex.width, charCamTex.height), 0, 0);
        charCamTex.Apply();

        worldCamera.Render();
        Texture2D worldCamTex = new Texture2D(256, 256);
        RenderTexture.active = worldCamera.activeTexture;
        worldCamTex.ReadPixels(new Rect(0, 0, worldCamTex.width, worldCamTex.height), 0, 0);
        worldCamTex.Apply();

        int charCount = 0;
        int hitCount = 0;
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                if (!(characterCamera.backgroundColor == charCamTex.GetPixel(x, y)))
                {
                    charCount++;
                    //if (worldCamTex.GetPixel(x, y) == charCamTex.GetPixel(x, y))
                    if((worldCamTex.GetPixel(x,y).r-charCamTex.GetPixel(x,y).r<.05 && worldCamTex.GetPixel(x, y).r - charCamTex.GetPixel(x, y).r > -.05) &&
                       (worldCamTex.GetPixel(x,y).g-charCamTex.GetPixel(x,y).g<.05 && worldCamTex.GetPixel(x, y).g - charCamTex.GetPixel(x, y).g > -.05) &&
                       (worldCamTex.GetPixel(x,y).b-charCamTex.GetPixel(x,y).b<.05 && worldCamTex.GetPixel(x, y).b - charCamTex.GetPixel(x, y).b > -.05) &&
                       (worldCamTex.GetPixel(x,y).a-charCamTex.GetPixel(x,y).a<.05 && worldCamTex.GetPixel(x, y).a - charCamTex.GetPixel(x, y).a > -.05))
                    {
                        hitCount++;
                    }
                }
            }
        }
        setLayer(beforeLayer, target.transform);
        int returnVal = (int)(100 * (hitCount / ((float)charCount)));
        print(hitCount + " " + charCount + " " + returnVal);
        return returnVal;
    }
}
