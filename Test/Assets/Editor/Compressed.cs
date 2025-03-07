using UnityEditor;
using UnityEngine;

public class Compressed : MonoBehaviour
{
    private static TextureImporter importer = null;//纹理导入器
    
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
        foreach (var obj in Selection.objects)
        {
            if (!(obj is Texture2D)) //跳过非图片文件
                continue;

            string path = AssetDatabase.GetAssetPath(obj);//记录当前文件路径
            importer = AssetImporter.GetAtPath(path) as TextureImporter;//获取path路径资源

            if (importer != null)
            {
                TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(platform);//指定平台
                settings.overridden = true;
                settings.format = format;
                settings.compressionQuality = (int)quality;
                importer.SetPlatformTextureSettings(settings);

                EditorUtility.SetDirty(importer);//标记为脏，表示修改已保存
                importer.SaveAndReimport();//应用修改apply
            }
        }
        AssetDatabase.Refresh(); //刷新资源数据库         
    }    
}