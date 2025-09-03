/// <summary>
/// Appクラスは遺伝的アルゴリズム(GA)による個体集団の管理・進化・描画・UI連携を担当します。
/// - GAコンポーネントで集団の進化を制御
/// - DrawingManegerで個体の描画
/// - UIから評価値を取得し進化に反映
/// - 世代数や進化ボタンの管理
/// </summary>

using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 個体クラス：遺伝子情報を保持
public class Individual
{
    public int[] gene;
}

// アプリケーションのメイン制御クラス
public class App : MonoBehaviour
{
    [Header("必須コンポーネント")]
    [Tooltip("遺伝的アルゴリズムを担当するGAコンポーネント")]
    public GA ga;

    [Tooltip("個体の描画を担当するDrawingManegerコンポーネント")]
    public DrawingManeger[] drawingManagers;

    [Header("UI要素")]
    public List<GameObject> dropdownObjList = new List<GameObject>(); // 評価用ドロップダウンリスト
    public GameObject nextGenerationButton; // 次世代生成ボタン
    public GameObject canvas; // UIキャンバス
    public GameObject[] objBGs; // 背景オブジェクト
    public TextMeshProUGUI generationText; // 世代表示テキスト

    private Individual[] population; // 現在の集団
    private int[] fitnessValues; // 評価値配列

    // 初期化処理
    void Start()
    {
        if (!ValidateComponents()) return; // 必須コンポーネントの確認

        InitializePopulation(); // 集団の初期化
        ga.Initialize(population); // GAの初期化
        UpdateGenerationText(); // 世代表示の更新
        DrawInitialPopulation(); // 初期集団の描画
    }

    // 次世代の描画・進化処理
    public void RenderNextGeneration()
    {
        int[] scores = GetDropdownScores(); // UIから評価値取得
        LogScores(scores); // 評価値のログ出力

        population = ga.Evolve(scores); // GAで進化
        UpdateGenerationText(); // 世代表示の更新
        DrawPopulation(); // 新しい集団の描画
        ResetDropdowns(); // ドロップダウンのリセット
        CheckGenerationLimit(); // 世代数の上限チェック
    }

    // 必須コンポーネントの確認
    private bool ValidateComponents()
    {
        if (ga == null)
        {
            Debug.LogError("GAスクリプトがインスペクターで設定されていません！", gameObject);
            return false;
        }
        if (drawingManagers == null)
        {
            Debug.LogError("DrawingManegerスクリプトがインスペクターで設定されていません！", gameObject);
            return false;
        }
        return true;
    }

    // 集団の初期化
    private void InitializePopulation()
    {
        int popSize = ga.POP_SIZE;
        population = new Individual[popSize];
        fitnessValues = new int[popSize];
        for (int i = 0; i < popSize; ++i)
        {
            population[i] = new Individual();
        }
    }

    // 初期集団の描画
    private void DrawInitialPopulation()
    {
        if (population == null || population.Length == 0) return;
        Debug.Log("初期集団の最初の個体を描画します。");
        for (int i = 0; i < drawingManagers.Length && i < population.Length; i++)
        {
            drawingManagers[i].DrawImageFromGene(population[i].gene);
        }
    }

    // UIドロップダウンから評価値を取得
    private int[] GetDropdownScores()
    {
        int[] scores = new int[ga.POP_SIZE];
        for (int i = 0; i < dropdownObjList.Count && i < scores.Length; i++)
        {
            TMP_Dropdown dropdown = dropdownObjList[i].GetComponent<TMP_Dropdown>();
            scores[i] = dropdown.value;
        }
        return scores;
    }

    // 評価値のログ出力
    private void LogScores(int[] scores)
    {
        for (int j = 0; j < scores.Length; j++)
        {
            Debug.Log(scores[j]);
        }
    }

    // 集団の描画
    private void DrawPopulation()
    {
        for (int k = 0; k < population.Length && k < drawingManagers.Length; k++)
        {
            drawingManagers[k].DrawImageFromGene(population[k].gene);
        }
    }

    // ドロップダウンのリセット
    private void ResetDropdowns()
    {
        foreach (var dropdownObj in dropdownObjList)
        {
            if (dropdownObj == null) continue;
            TMP_Dropdown dropdown = dropdownObj.GetComponent<TMP_Dropdown>();
            if (dropdown != null)
            {
                dropdown.value = 0;
            }
        }
    }

    // 世代表示の更新
    private void UpdateGenerationText()
    {
        if (generationText != null && ga != null)
        {
            generationText.text = $"{ga.currentGeneration}";
        }
    }

    // 世代数の上限チェック
    private void CheckGenerationLimit()
    {
        if (ga != null && nextGenerationButton != null && ga.currentGeneration >= ga.MAX_GEN)
        {
            nextGenerationButton.SetActive(false);
        }
    }
}

