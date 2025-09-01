using System.IO;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class ReferenceImage : MonoBehaviour
{
    [MenuItem("GameObject/添加参考图", false, -100)]

    static void MyTools()
    {
        if (Selection.activeObject != null)
        {
            spr_path = EditorUtility.OpenFilePanel("加载外部图片","", "");

            if(spr_path.Length!=0)
            {
                Debug.Log("生成效果图");
                
                GameObject newObj = new GameObject("效果图");
                newObj.transform.SetParent(Selection.activeTransform,false);
                Image imageComponent = newObj.AddComponent<Image>();
                
                byte[] imgBuff = File.ReadAllBytes(spr_path);
            
                /*byte[] imgBuff = File.ReadAllBytes("Assets/DoctorUpgradeGift/Texture/UI_DoctorUpgradeGift_Arrow.png");*/
                Texture2D texture2D = new Texture2D(1, 1);//默认尺寸是多少都无所谓
                texture2D.LoadImage(imgBuff);
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = sprite;
            
                imageComponent.SetNativeSize();
            
                Color Imagecolor =imageComponent.GetComponent<Image>().color;
                Imagecolor.a = 0.7f;
                imageComponent.GetComponent<Image>().color =Imagecolor;
            }
            else
            {
                Debug.Log("未选择图片");
                /*GameObject newObj = new GameObject("Image");
                newObj.transform.SetParent(Selection.activeTransform,false);
                Image imageComponent = newObj.AddComponent<Image>();*/
            }
        }
    }

    static string spr_path = "";
    
}