using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExportUI
{
    [MenuItem("Tools/导出选中UI层级")]
    public static void ExportSelection()
    {
        // 确保 Hierarchy 里选中了 UI 节点
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("导出失败：请先在 Hierarchy 窗口中选择要导出的 UI 根节点！");
            return;
        }
        
        // 1. 获取选中的 UI 根节点名称作为文件名
        string uiName = Selection.activeGameObject.name;

        // 2. 默认一个导出路径（防止 Project 窗口没有高亮任何文件夹）
        string targetFolder = "Assets"; 
        
        // 3. 获取当前在 Project 窗口中鼠标选中的目录
        foreach (var obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(assetPath))
            {
                // 如果选中的是文件，就取它所在的文件夹路径；如果本身就是文件夹，直接使用
                if (Directory.Exists(assetPath))
                {
                    targetFolder = assetPath;
                    break;
                }
                else if (File.Exists(assetPath))
                {
                    targetFolder = Path.GetDirectoryName(assetPath);
                    break;
                }
            }
        }

        // 4. 按照新格式开始拼接头部信息
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("功能描述：");
        sb.AppendLine(); // 空一行
        sb.AppendLine("层级结构与节点作用：");

        // 5. 递归解析层级并追加到文本中
        ParseTransform(Selection.activeGameObject.transform, sb, 0);
        
        // 6. 动态拼接文件名并写入到选中的 Project 目录下
        string fileName = $"{uiName}.txt";
        string fullPath = Path.Combine(Application.dataPath, "../", targetFolder, fileName);
        File.WriteAllText(fullPath, sb.ToString());
        
        AssetDatabase.Refresh(); // 强制让 Unity 刷新 Project 视图
        Debug.Log($"<color=#00ff00>【导出成功】</color> 结构文档已成功生成至: <color=#ffaa00>{targetFolder}/{fileName}</color>");
    }

    private static void ParseTransform(Transform trans, StringBuilder sb, int depth)
    {
        // 严格按照你要求的 4 个普通空格作为每层的缩进
        string indent = new string(' ', depth * 4); 
        
        // 判断节点显隐状态（对应 Prefab 源码中的 m_IsActive）
        string activeStr = trans.gameObject.activeSelf ? "" : "[]（关闭）";
        
        // 自动识别常见的组件类型
        string compType = "GameObject";
        if (trans.GetComponent<UnityEngine.UI.Button>()) compType = "Button";
        else if (trans.GetComponent<TMPro.TextMeshProUGUI>()) compType = "TextMeshProUGUI";
        else if (trans.GetComponent<UnityEngine.UI.Image>()) compType = "Image";
        else if (trans.GetComponent<UnityEngine.UI.Toggle>()) compType = "Toggle";
        else if (trans.GetComponent<UnityEngine.UI.Slider>()) compType = "Slider";
        else if (trans.GetComponent<UnityEngine.UI.ScrollRect>()) compType = "ScrollRect";

        // 按照你指定的指令格式拼接：- 节点名 [组件类型](作用说明)
        sb.AppendLine($"{indent}- {trans.name} [{compType}]{activeStr}(功能)");//请AI帮忙补充节点作用说明

        // 递归处理子节点
        for (int i = 0; i < trans.childCount; i++)
        {
            ParseTransform(trans.GetChild(i), sb, depth + 1);
        }
    }
}