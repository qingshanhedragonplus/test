using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ResourceReferenceFinder
{
    private static List<string> referencingFiles = new List<string>();
    private static string selectedAssetPath;
    private static bool isSearching;
    private static bool isCancelled;

    [MenuItem("Assets/查找该资源引用", false, 104)]
    private static void FindResourceReferences()
    {
        // 确保只选中了一个资源
        if (Selection.objects.Length != 1)
        {
            EditorUtility.DisplayDialog("提示", "请选择一个资源进行查找", "确定");
            return;
        }

        // 获取选中的资源路径
        selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(selectedAssetPath))
        {
            EditorUtility.DisplayDialog("错误", "无法获取资源路径", "确定");
            return;
        }

        // 重置状态
        referencingFiles.Clear();
        isSearching = true;
        isCancelled = false;

        // 开始搜索（使用延迟调用避免编辑器卡顿）
        EditorApplication.delayCall += StartSearch;
    }

    private static void StartSearch()
    {
        try
        {
            // 获取项目中所有需要检查的文件
            string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(f => !f.EndsWith(".meta") && 
                            !f.EndsWith(".cs") && 
                            !f.EndsWith(".js") && 
                            !f.EndsWith(".shader") && 
                            !f.EndsWith(".cginc"))
                .ToArray();

            // 获取目标资源的GUID（用于在文件中查找）
            string targetGUID = AssetDatabase.AssetPathToGUID(selectedAssetPath);

            // 显示进度条
            int totalFiles = allFiles.Length;
            int processed = 0;

            foreach (string filePath in allFiles)
            {
                if (isCancelled) break;

                // 更新进度条
                float progress = (float)processed / totalFiles;
                if (EditorUtility.DisplayCancelableProgressBar(
                    "引用检查中", 
                    $"正在检查: {Path.GetFileName(filePath)}", 
                    progress))
                {
                    isCancelled = true;
                    break;
                }

                // 将绝对路径转换为Unity资源路径
                string assetPath = "Assets" + filePath.Replace(Application.dataPath, "").Replace('\\', '/');

                // 跳过目标资源本身
                if (assetPath == selectedAssetPath)
                {
                    processed++;
                    continue;
                }

                // 检查文件是否包含目标资源的GUID
                string fileContent = File.ReadAllText(filePath);
                if (fileContent.Contains(targetGUID))
                {
                    referencingFiles.Add(assetPath);
                }

                processed++;
            }
        }
        finally
        {
            // 完成搜索，清理进度条
            EditorUtility.ClearProgressBar();
            isSearching = false;

            // 显示结果
            DisplayResults();
        }
    }

    private static void DisplayResults()
    {
        // 清除控制台
        Debug.ClearDeveloperConsole();

        // 输出被引用资源的目录
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        UnityEngine.Object obj2 = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        //Debug.Log("被引用资源的目录："+path);
        Debug.Log($"被引用资源的目录："+path,obj2);

        // 输出所有引用文件
        if (referencingFiles.Count > 0)
        {
            foreach (string filePath in referencingFiles)
            {
                // 加载资源以便在控制台创建可点击链接
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
                
                if (obj != null)
                {
                    // 使用Debug.Log输出带有上下文对象的日志
                    // 在控制台点击消息时会选中该对象
                    //方法一
                    //Debug.Log($"{Path.GetDirectoryName(filePath)}\\{Path.GetFileName(filePath)} 引用了该资源", obj);
                    
                    //方法二
                    /*string objpath=Path.GetDirectoryName(filePath);
                    string objname=Path.GetFileName(filePath);
                    Debug.Log(objpath+$"\\"+objname+$" 引用了该资源",obj);*/
                    
                    //方法三
                    Debug.Log(filePath+$" 引用了该资源",obj);
                }
                else
                {
                    // 对于无法加载的资源，直接输出路径
                    Debug.Log($"\"{Path.GetDirectoryName(filePath)}\" 引用了该资源 (无法加载资源)");
                }
            }
            Debug.Log("已显示所有引用");
        }
        else
        {
            Debug.Log("未找到引用该资源的文件");
        }
    }
}
