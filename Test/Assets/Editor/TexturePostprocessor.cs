using UnityEditor;
using UnityEngine;

public class TexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 获取当前纹理导入器
        var importer = (TextureImporter)assetImporter;
        
        // 通用设置：所有图片设为Sprite模式
        importer.textureType = TextureImporterType.Sprite;

        // 特殊处理Spine资源
        if (importer.assetPath.Contains("Spine/")) // 根据实际路径调整
        {
            importer.alphaIsTransparency = false;
            Debug.Log($"Processed Spine texture: {importer.assetPath}");
        }
    }
}