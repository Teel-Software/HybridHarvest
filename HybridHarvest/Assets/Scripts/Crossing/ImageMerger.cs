using System.IO;
using UnityEngine;

public static class ImageMerger
{
    public static void MergeParentImages(string parentName1, string parentName2)
    {
        var path = Path.Combine(Application.persistentDataPath, parentName1 + "-" + parentName2 + ".png");
        if (File.Exists(path)) return;
        Texture2D im1;
        Texture2D im2;
        try
        {
            im1 = Resources.Load<Texture2D>($"SeedsIcons\\{parentName1}");
            im1 = duplicateTexture(im1);
        }
        catch 
        {
            //using (var req = UnityWebRequestTexture.GetTexture("file:///" + Path.Combine(Application.persistentDataPath, parentName1 + ".png")))
            //{
            //    //req.downloadHandler = new DownloadHandlerTexture();
            //    Debug.Log(req);
            //    req.SendWebRequest();
            //    Debug.Log(req);
            //    Debug.Log(req.isDone);
            //    while (!req.isDone)
            //    {
            //        Debug.Log(req.downloadProgress);
            //    }
            //    im1 = DownloadHandlerTexture.GetContent(req);
            //}
            var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, parentName1 + "Texture.png"));
            im1 = req.texture;
        }
        try
        {
            im2 = Resources.Load<Texture2D>($"SeedsIcons\\{parentName2}Texture");
            im2 = duplicateTexture(im2);
        }
        catch
        {
            var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, parentName2 + "Texture.png"));
            im2 = req.texture;
        }        

        var res = duplicateTexture(im2);
        //res.Resize(im2.width + 2, im2.height + 2);

        //for (int i = 0; i < im1.width + 2; i++)
        //{
        //    res.SetPixel(i, 0, new Color(0f, 0f, 0f, 0.2f));
        //    res.SetPixel(i, im1.width + 1, new Color(0f, 0f, 0f, 0.2f));
        //}
        //for (int i = 0; i < im1.height + 2; i++)
        //{
        //    res.SetPixel(0, i, new Color(0f, 0f, 0f, 0.2f));
        //    res.SetPixel(im1.height +1, i, new Color(0f, 0f, 0f, 0.2f));
        //}

        for (int i = 0; i< im1.width; i++)
        {
            for (int j = 0; j < im1.height; j++)
            {
                var pix = im1.GetPixel(i, j);
                if (pix.a == 0)
                    res.SetPixel(i/*+1*/, j/*+1*/, pix);
                else
                    res.SetPixel(i /*+ 1*/, j /*+ 1*/, im2.GetPixel(i, j));
            }
        }
        res.filterMode = FilterMode.Point;
        File.WriteAllBytes(path, res.EncodeToPNG());
    }

    private static Texture2D duplicateTexture(Texture2D source)
    {
        var renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

}
