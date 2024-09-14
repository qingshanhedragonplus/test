using System;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 资源重设置导入刷新
/// </summary>
public class AssetProcessor : AssetPostprocessor
{
    [MenuItem("Assets/Texture/SetAllImagesToSpriteType")]
    public static void SetAllTextureType()
    {
        //获取鼠标点击图片目录
        var arr = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
        string folder = AssetDatabase.GetAssetPath(arr[0]);
        Debug.Log("SetAllImagesToSpriteType Path:" + folder);

        //遍历文件夹获取所有.png/.jpg
        DirectoryInfo direction = new DirectoryInfo(folder);
        FileInfo[] pngFiles = direction.GetFiles("*.png", SearchOption.AllDirectories);
        FileInfo[] jpgfiles = direction.GetFiles("*.jpg", SearchOption.AllDirectories);

        try
        {
            SetTexture(pngFiles);
            SetTexture(jpgfiles);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    static void SetTexture(FileInfo[] fileInfo)
    {
        for (int i = 0; i < fileInfo.Length; i++)
        {
            //这里第一次写时有一个接口可直接调用，但是第二次写时找不到了 所以用了切割字符
            string fullpath = fileInfo[i].FullName.Replace("\\", "/");
            string path = fullpath.Replace(Application.dataPath, "Assets");
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            EditorUtility.DisplayProgressBar("批量处理图片", $"{i}/{fileInfo.Length}  \n{fileInfo[i].Name} ", i / (float)fileInfo.Length);
            SetTextureFormat(textureImporter);
            //触发重新导入资源才生效，否则需要重启Unity触发自动导入
            AssetDatabase.ImportAsset(path);
        }
    }

    //设置图片格式
    static void SetTextureFormat(TextureImporter textureImporter)
    {
        //根据路径获得文件夹目录，设置图集的packagingTag
        string AtlasName = new DirectoryInfo(Path.GetDirectoryName(textureImporter.assetPath)).Name;
        textureImporter.mipmapEnabled = false;
        textureImporter.isReadable = false;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spritePackingTag = AtlasName;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
        textureImporter.textureFormat = TextureImporterFormat.Automatic;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;

        //Android端单独设置
        TextureImporterPlatformSettings setting_android = new TextureImporterPlatformSettings();
        setting_android.overridden = true;
        setting_android.name = "Android";
        //根据是否有透明度，选择RGBA还是RGB
        /*if (textureImporter.DoesSourceTextureHaveAlpha())
            setting_android.format = TextureImporterFormat.ETC2_RGBA8;
        else
            setting_android.format = TextureImporterFormat.ETC2_RGB4;*/

        setting_android.format = TextureImporterFormat.RGBA32;

        textureImporter.SetPlatformTextureSettings(setting_android);

        //IOS端单独设置
        TextureImporterPlatformSettings setting_iphone = new TextureImporterPlatformSettings();
        setting_iphone.overridden = true;
        setting_iphone.name = "iOS";
        /*//根据是否有透明度，选择RGBA还是RGB
        if (textureImporter.DoesSourceTextureHaveAlpha())
            setting_android.format = TextureImporterFormat.ASTC_6x6;
        else
            setting_android.format = TextureImporterFormat.ASTC_6x6;*/
        
        setting_iphone.format = TextureImporterFormat.RGBA32;
        
        textureImporter.SetPlatformTextureSettings(setting_iphone);
        
        //Deault设置
        TextureImporterPlatformSettings setting_deault = new TextureImporterPlatformSettings();
        setting_deault.overridden = true;
        setting_deault.name = "Default";
       
        setting_deault.format = TextureImporterFormat.RGBA32;
        
        textureImporter.SetPlatformTextureSettings(setting_deault);
    }
}