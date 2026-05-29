using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    // 按钮组件
    private Button m_CloseBtn;
    private Button m_5_Btn;
    private Button m_4_Btn;
    private Button m_3_Btn;
    private Button m_PlayBtn;
    private Button m_PlayGrayBtn;
    private Button m_ReliveBtn;

    // 星星节点与星星物体的数组（长度为5）
    private GameObject[] m_StarBGs = new GameObject[5];
    private GameObject[] m_Stars = new GameObject[5];

    private void Awake()
    {
        // 1. 自动查找并绑定所有节点
        FindAndBindReferences();

        // 2. 初始化按钮点击事件
        InitButtons();

        // 3. 初始化界面状态
        InitUIState();
    }

    /// <summary>
    /// 根据严格的层级结构，通过路径自动查找组件
    /// </summary>
    private void FindAndBindReferences()
    {
        // 查找关闭按钮
        m_CloseBtn = transform.Find("Root/m_ColseBtn").GetComponent<Button>();

        // 查找数量按钮组
        m_5_Btn = transform.Find("Root/NumGroup/m_5_Btn").GetComponent<Button>();
        m_4_Btn = transform.Find("Root/NumGroup/m_4_Btn").GetComponent<Button>();
        m_3_Btn = transform.Find("Root/NumGroup/m_3_Btn").GetComponent<Button>();

        // 查找功能按钮组
        m_PlayBtn = transform.Find("Root/ButtonGroup/m_PlayBtn").GetComponent<Button>();
        m_PlayGrayBtn = transform.Find("Root/ButtonGroup/m_PlayGrayBtn").GetComponent<Button>();
        m_ReliveBtn = transform.Find("Root/ButtonGroup/m_ReliveBtn").GetComponent<Button>();

        // 循环自动绑定 5 个星星节点和星星图片
        for (int i = 0; i < 5; i++)
        {
            int num = i + 1; // 对应名称中的 1-5
            
            // 查找星星背景节点 (如: Root/StarGroup/m_StarBG1)
            Transform bgTransform = transform.Find($"Root/StarGroup/m_StarBG{num}");
            if (bgTransform != null)
            {
                m_StarBGs[i] = bgTransform.gameObject;

                // 查找背景节点下的具体星星图片 (如: m_Star1)
                Transform starTransform = bgTransform.Find($"m_Star{num}");
                if (starTransform != null)
                {
                    m_Stars[i] = starTransform.gameObject;
                }
            }
        }
    }

    // 初始化按钮点击事件
    private void InitButtons()
    {
        // 点击关闭当前界面
        m_CloseBtn.onClick.AddListener(CloseUI);

        // 点击按钮打开对应数量的星星节点
        m_5_Btn.onClick.AddListener(() => OpenStarNodes(5));
        m_4_Btn.onClick.AddListener(() => OpenStarNodes(4));
        m_3_Btn.onClick.AddListener(() => OpenStarNodes(3));

        // 核心功能按钮
        m_PlayBtn.onClick.AddListener(OnPlayClicked);
        m_ReliveBtn.onClick.AddListener(OnReliveClicked);
    }

    // 初始化界面状态
    private void InitUIState()
    {
        // 初始时默认关闭所有星星节点
        foreach (var bg in m_StarBGs)
        {
            if (bg != null) bg.SetActive(false);
        }

        // 初始时 Play 可用，PlayGray 关闭
        m_PlayBtn.gameObject.SetActive(true);
        m_PlayGrayBtn.gameObject.SetActive(false);
    }

    // 关闭当前界面
    private void CloseUI()
    {
        gameObject.SetActive(false);
    }

    // 打开指定数量的星星节点，并默认把里面的星星点亮
    private void OpenStarNodes(int count)
    {
        for (int i = 0; i < m_StarBGs.Length; i++)
        {
            if (m_StarBGs[i] == null) continue;

            bool shouldOpen = i < count;
            m_StarBGs[i].SetActive(shouldOpen);
            
            // 打开节点时，默认把里面的星星也重置为显示状态
            if (shouldOpen && m_Stars[i] != null)
            {
                m_Stars[i].SetActive(true);
            }
        }

        UpdateButtonState();
    }

    // 点击 Play 按钮：从后往前关闭一个“当前已打开节点”下的星星
    private void OnPlayClicked()
    {
        // 从最后一个星星开始往前找，关闭第一个处于显示状态的星星
        for (int i = m_Stars.Length - 1; i >= 0; i--)
        {
            if (m_StarBGs[i] == null || m_Stars[i] == null) continue;

            // 条件：星星节点是打开的，且里面的星星也是显示状态
            if (m_StarBGs[i].activeSelf && m_Stars[i].activeSelf)
            {
                m_Stars[i].SetActive(false);
                break; // 每次只关闭一个
            }
        }

        UpdateButtonState();
    }

    // 点击 Relive 按钮：将所有【已打开节点】下的星星重新点亮
    private void OnReliveClicked()
    {
        for (int i = 0; i < m_Stars.Length; i++)
        {
            if (m_StarBGs[i] != null && m_StarBGs[i].activeSelf && m_Stars[i] != null)
            {
                m_Stars[i].SetActive(true);
            }
        }

        UpdateButtonState();
    }

    // 统一管理 Play 和 PlayGray 按钮的显隐状态
    private void UpdateButtonState()
    {
        bool hasActiveStar = false;

        // 遍历所有打开的节点，看看里面还有没有亮着的星星
        for (int i = 0; i < m_StarBGs.Length; i++)
        {
            if (m_StarBGs[i] != null && m_Stars[i] != null)
            {
                if (m_StarBGs[i].activeSelf && m_Stars[i].activeSelf)
                {
                    hasActiveStar = true;
                    break;
                }
            }
        }

        // 如果还有星星亮着，Play 激活；如果全灭，Play 隐藏，PlayGray 显示
        m_PlayBtn.gameObject.SetActive(hasActiveStar);
        m_PlayGrayBtn.gameObject.SetActive(!hasActiveStar);
    }
}