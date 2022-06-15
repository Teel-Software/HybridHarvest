using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ImageMerger
{
    private static Texture2D reserve;
    public static void MergeParentImages(List<string> parents)
    {
        var newName = string.Join("-", parents);

        var check = Resources.Load<Texture2D>($"SeedsIcons\\{newName}");
        if (check != null) return;

        var spritePath = Path.Combine(Application.persistentDataPath, newName + ".png");
        if (File.Exists(spritePath)) return;

        var shapeName = parents[newName.ToCharArray().Length % parents.Count]; // за форму овоща отвечает (кол-во букв % кол-во родителей)

        var shadow = Resources.Load<Texture2D>($"SeedsIcons\\{shapeName}Shadow");
        shadow = duplicateTexture(shadow);

        var shape = Resources.Load<Texture2D>($"SeedsIcons\\{shapeName}");
        shape = duplicateTexture(shape);
        //var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, formName + ".png"));
        //formForTexture = req.texture;

        reserve = Resources.Load<Texture2D>($"SeedsIcons\\{shapeName}Texture");
        reserve = duplicateTexture(reserve);
        var texture = mergeTextures(parents.Where(x => x != shapeName).ToList());
        var newSprite = duplicateTexture(texture);
     


        for (int i = 0; i < shape.width; i++)
        {
            for (int j = 0; j < shape.height; j++)
            {
                var formCol = shape.GetPixel(i, j);
                var textureCol = texture.GetPixel(i, j);
                var shadowCol = shadow.GetPixel(i, j);
                var newColor = new Color((textureCol.r + shadowCol.r) / 2,
                    (textureCol.g + shadowCol.g) / 2,
                    (textureCol.b + shadowCol.b) / 2,
                    textureCol.a);

                if (formCol.a == 0)
                    newSprite.SetPixel(i, j, formCol);
                else if (shadowCol.a != 0)
                    newSprite.SetPixel(i, j, newColor);
                else                                                                       //без else текстура остаётся неизменённой
                    newSprite.SetPixel(i, j, textureCol);
            }
        }
        newSprite.filterMode = FilterMode.Point;
        File.WriteAllBytes(spritePath, newSprite.EncodeToPNG());
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

    /// <summary>
    /// Возвращает текстуру, собранную из родителей
    /// </summary>
    private static Texture2D mergeTextures(List<string> parents)
    {
        if (parents.Count < 1 || parents.Count > 4)
        {
            Debug.LogError("wth, man!? tis aint right");  //Лучшее в мире оповещение об ошибке
            return reserve;
        }

        var texturesToUse = new List<Texture2D>();
        foreach (var name in parents)
            texturesToUse.Add(duplicateTexture(Resources.Load<Texture2D>($"SeedsIcons\\{name}Texture")));

        var newTexture = texturesToUse[0];

        if (parents.Count > 1)
        {
            for (int i = 0; i < newTexture.width; i++)
            {
                for (int j = 0; j < newTexture.height / 2; j++)
                {
                    var pix = texturesToUse[1].GetPixel(i, j);
                    newTexture.SetPixel(i, j, pix);
                }
            }
        }
        if (parents.Count > 2)
        {
            for (int i = newTexture.width / 2; i < newTexture.width; i++)
            {
                for (int j = 0; j < newTexture.height / 2; j++)
                {
                    var pix = texturesToUse[2].GetPixel(i, j);
                    newTexture.SetPixel(i, j, pix);
                }
            }
        }
        if (parents.Count > 3)
        {
            for (int i = 0; i < newTexture.width / 2; i++)
            {
                for (int j = newTexture.height / 2; j < newTexture.height; j++)
                {
                    var pix = texturesToUse[3].GetPixel(i, j);
                    newTexture.SetPixel(i, j, pix);
                }
            }
        }

        return newTexture;
    }
}
