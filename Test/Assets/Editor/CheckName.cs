using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class CheckName : MonoBehaviour
{
    [MenuItem("GameObject/检查是否有重复命名", false, -101)]
    private static void CheckDuplicateNames()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            Debug.Log("没有选中任何节点！");
            return;
        }

        Dictionary<string, List<GameObject>> nameMap = new Dictionary<string, List<GameObject>>();
        List<string> duplicateNames = new List<string>();
        StringBuilder result = new StringBuilder();
        
        // 收集所有节点信息
        foreach (GameObject obj in selectedObjects)
        {
            string name = obj.name;
            
            if (nameMap.ContainsKey(name))
            {
                nameMap[name].Add(obj);
                if (!duplicateNames.Contains(name)) duplicateNames.Add(name);
            }
            else
            {
                nameMap[name] = new List<GameObject> { obj };
            }
        }

        if (duplicateNames.Count == 0)
        {
            result.AppendLine("节点命名无重复");
            result.AppendLine();
            result.AppendLine($"已检查节点总数：{selectedObjects.Length}");
            Debug.Log(result.ToString());
        }
        else
        {
            // 打印重复节点名列表
            result.Append("发现重复命名的节点：");
            for (int i = 0; i < duplicateNames.Count; i++)
            {
                result.Append($"<color=yellow>{duplicateNames[i]}</color>");
                if (i < duplicateNames.Count - 1) result.Append("、");
            }
            result.AppendLine("\n");
            
            // 打印每个重复节点的详细信息
            foreach (string name in duplicateNames)
            {
                result.AppendLine($"名称：<color=yellow>{name}</color>（出现次数：{nameMap[name].Count}）");
                result.AppendLine("路径：");
                
                foreach (GameObject obj in nameMap[name])
                {
                    result.AppendLine(GetFullPath(obj));
                }
                result.AppendLine();
            }
            
            // 打印总数
            result.AppendLine($"已检查节点总数：{selectedObjects.Length}");
            Debug.Log(result.ToString());
        }
    }
    
    // 获取节点的完整路径
    private static string GetFullPath(GameObject obj)
    {
        if (obj == null) return "";
        
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}
