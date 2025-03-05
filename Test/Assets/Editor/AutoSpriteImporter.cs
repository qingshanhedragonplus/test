using UnityEditor;
using UnityEngine;

public class AutoSpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 获取当前纹理导入器
        var importer = (TextureImporter)assetImporter;
        
        // 排除非2D用途的纹理（可选）
        if (importer.assetPath.Contains("3DTextures")) return;

        // 配置Sprite参数
        importer.textureType = TextureImporterType.Sprite;
        
        importer.spritePixelsPerUnit = 100; // 标准像素单位比
        importer.mipmapEnabled = false;     // 禁用Mipmap
        
        // 设置默认九宫格边界（根据项目需求调整）
        //importer.spriteBorder = new Vector4(2, 2, 2, 2);
        
        // 配置高级参数
        importer.spritePivot = new Vector2(0.5f, 0.5f); // 中心轴点
        importer.spriteImportMode = SpriteImportMode.Single; // 单图模式
        importer.maxTextureSize = 2048; // 最大纹理尺寸
    }
}
