using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Compressed : EditorWindow
{
    private static TextureImporter importer = null;
    
    [MenuItem("Assets/图片压缩格式/6x6", false, 102)]
    static void Compress_6()
    {
        SetCompress(importer, "Android", 
            TextureImporterFormat.ASTC_6x6, 
            TextureCompressionQuality.Best);
        
        SetCompress(importer, "iPhone", 
            TextureImporterFormat.ASTC_6x6, 
            TextureCompressionQuality.Best);
    }
    
    [MenuItem("Assets/图片压缩格式/5x5", false, 101)]
    static void Compress_5()
    {
        SetCompress(importer, "Android", 
            TextureImporterFormat.ASTC_5x5, 
            TextureCompressionQuality.Best);
        
        SetCompress(importer, "iPhone", 
            TextureImporterFormat.ASTC_5x5, 
            TextureCompressionQuality.Best);
    }
    
    [MenuItem("Assets/图片压缩格式/4x4", false, 100)]
    static void Compress_4()
    {
        SetCompress(importer, "Android", 
            TextureImporterFormat.ASTC_4x4, 
            TextureCompressionQuality.Best);
        
        SetCompress(importer, "iPhone", 
            TextureImporterFormat.ASTC_4x4, 
            TextureCompressionQuality.Best);
    }

    static void SetCompress(TextureImporter importer, string platform, 
        TextureImporterFormat format, TextureCompressionQuality quality)
    {
        List<string> processedAssets = new List<string>();
        foreach (var obj in Selection.objects)
        {
            if (!(obj is Texture2D)) continue;

            string path = AssetDatabase.GetAssetPath(obj);
            importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(platform);
                settings.overridden = true;
                settings.format = format;
                settings.compressionQuality = (int)quality;
                importer.SetPlatformTextureSettings(settings);

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                processedAssets.Add(path);
            }
        }
        AssetDatabase.Refresh();          
    }    
}