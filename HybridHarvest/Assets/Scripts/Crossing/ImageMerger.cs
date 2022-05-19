using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ImageMerger
{
    public static void MergeParentImages(string parentName1, string parentName2, List<string> fp, List<string> sp)
    {
        var path = Path.Combine(Application.persistentDataPath, parentName1 + "-" + parentName2 + ".png");
        var texturePath = Path.Combine(Application.persistentDataPath, parentName1 + "-" + parentName2 + "Texture.png");
        if (File.Exists(path)) return;
        Texture2D im1;
        Texture2D im2;
        Texture2D tex1;
        try
        {
            tex1 = Resources.Load<Texture2D>($"SeedsIcons\\{parentName1}Texture");
            tex1 = duplicateTexture(tex1);
        }
        catch
        {
            var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, parentName1 + "Texture.png"));
            tex1 = req.texture;
        }
        try
        {
            im1 = Resources.Load<Texture2D>($"SeedsIcons\\{parentName1}");
            im1 = duplicateTexture(im1);
        }
        catch
        {
            var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, parentName1 + ".png"));
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
        var forma = parentName1.Split('-')[0];
        var newTex = mergeTextures(tex1, im2, fp.Where(x=> x!=forma).ToList(), sp);
        var res = duplicateTexture(newTex);

        for (int i = 0; i < im1.width; i++)
        {
            for (int j = 0; j < im1.height; j++)
            {
                var pix = im1.GetPixel(i, j);
                if (pix.a == 0)
                    res.SetPixel(i, j, pix);
                 else
                 res.SetPixel(i, j, newTex.GetPixel(i, j));
            }
        }
        res.filterMode = FilterMode.Point;
        File.WriteAllBytes(path, res.EncodeToPNG());
        File.WriteAllBytes(texturePath, newTex.EncodeToPNG());
    }

    /// <summary>
    /// Магия, не трогать!!!!!
    /// </summary>
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

    private static Texture2D mergeTextures(Texture2D tex1, Texture2D tex2, List<string> fp, List<string> sp)
    {
        if (fp.Count == 0 && sp.Count == 0)
            return tex2;
        var newTex = duplicateTexture(tex2);
        switch (sp.Count)
        {
            case 0:
                if (fp.Count == 1)
                {
                    for (int i = 0; i < tex2.width; i++)
                    {
                        for (int j = 0; j < tex2.height / 2; j++)
                        {
                            var pix = tex1.GetPixel(i, j);
                            newTex.SetPixel(i, j, pix);
                        }
                    }
                    return newTex;
                }
                if (fp.Count == 3)
                {
                    for (int i = 0; i < tex2.width; i++)
                    {
                        for (int j = 0; j < tex2.height / 2; j++)
                        {
                            var pix = tex1.GetPixel(i, j);
                            newTex.SetPixel(i, j, pix);
                        }
                    }
                    for (int i = 0; i < tex2.width/2; i++)
                    {
                        for (int j = tex2.height / 2; j < tex2.height; j++)
                        {
                            var pix = tex1.GetPixel(i, j);
                            newTex.SetPixel(i, j, pix);
                        }
                    }
                    return newTex;
                }
                for (int i = 0; i < tex2.width / 2; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = tex1.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = tex2.width / 2; i < tex2.width; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = tex1.GetPixel(j, i);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                return newTex;
            case 2:
                var p1 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[0]}Texture"));
                var p2 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[1]}Texture"));

                for (int i = 0; i < tex2.width; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = p1.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = 0; i < tex2.width; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = p2.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                if (fp.Count == 0) return newTex;
                var f1 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{fp[0]}Texture"));
                for (int i = 0; i < tex2.width / 2; i++)
                    {
                        for (int j = 0; j < tex2.height / 2; j++)
                        {
                            var pix = f1.GetPixel(i, j);
                            newTex.SetPixel(i, j, pix);
                        }
                    }
                if (fp.Count == 1) return newTex;
                var f2 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{fp[1]}Texture"));
                for (int i = 0; i < tex2.width / 2; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = f2.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                return newTex;
            case 3:
                var t1 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[0]}Texture"));
                var t2 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[1]}Texture"));
                var t3 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[2]}Texture"));
                for (int i = 0; i < tex2.width/2; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = t1.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = tex2.width / 2; i < tex2.width; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = t2.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = 0; i < tex2.width; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = t3.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                if (fp.Count == 0) return newTex;
                for (int i = 0; i < tex2.width / 2; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = tex1.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                return newTex;
            case 4:
                var ch1 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[0]}Texture"));
                var ch2 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[1]}Texture"));
                var ch3 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[2]}Texture"));
                var ch4 = duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{sp[3]}Texture"));
                for (int i = 0; i < tex2.width / 2; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = ch1.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = tex2.width / 2; i < tex2.width; i++)
                {
                    for (int j = 0; j < tex2.height / 2; j++)
                    {
                        var pix = ch2.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = 0; i < tex2.width/2; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = ch3.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                for (int i = tex2.width / 2; i < tex2.width; i++)
                {
                    for (int j = tex2.height / 2; j < tex2.height; j++)
                    {
                        var pix = ch4.GetPixel(i, j);
                        newTex.SetPixel(i, j, pix);
                    }
                }
                return newTex;
            default:
                return tex2;
        }
        return null;
    }
}
