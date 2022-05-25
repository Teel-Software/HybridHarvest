using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ImageMerger
{
    public static void MergeParentImages(List<string> firstParentsList, List<string> secondParentsList)
    {
        var newParentsList = firstParentsList.Concat(secondParentsList).OrderBy(x => x).ToList();
        var newName = string.Join("-", newParentsList);
        var spritePath = Path.Combine(Application.persistentDataPath, newName + ".png");
        var texturePath = Path.Combine(Application.persistentDataPath, newName + "Texture.png");
        if (File.Exists(spritePath)) return;
        firstParentsList.Sort(); secondParentsList.Sort();
        var parentName1 = string.Join("-", firstParentsList);
        var parentName2 = string.Join("-", secondParentsList);

        var formName = newParentsList[0]; // за форму овоща отвечает первый по алфавиту родитель
        Texture2D formForTexture;

        try
        {
            formForTexture = Resources.Load<Texture2D>($"SeedsIcons\\{formName}");
            formForTexture = duplicateTexture(formForTexture);
        }
        catch
        {
            var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, formName + ".png"));
            formForTexture = req.texture;
        }

        var newTexture = mergeTextures(newParentsList.Skip(1).ToList());
        var newSprite = duplicateTexture(newTexture);

        for (int i = 0; i < formForTexture.width; i++)
        {
            for (int j = 0; j < formForTexture.height; j++)
            {
                var pix = formForTexture.GetPixel(i, j);
                if (pix.a == 0)
                    newSprite.SetPixel(i, j, pix);
                else                                            //без else не работает(((
                    newSprite.SetPixel(i, j, newTexture.GetPixel(i, j));
            }
        }
        newSprite.filterMode = FilterMode.Point;
        File.WriteAllBytes(spritePath, newSprite.EncodeToPNG());
        File.WriteAllBytes(texturePath, newTexture.EncodeToPNG());
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
            return null;
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
