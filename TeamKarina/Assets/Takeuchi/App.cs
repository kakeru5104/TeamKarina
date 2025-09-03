using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// このクラスは「Individual.cs」という個別のファイルに分けるのが理想です。
public class Individual
{
    public int[] gene;

}


public class App : MonoBehaviour
{


    // インスペクターで設定するドロップダウンのゲームオブジェクトリスト
    public List<GameObject> dropdown_obj_list = new List<GameObject>();

    // ===== インスペクターから設定する参照 =====
    [Header("必須コンポーネント")]
    [Tooltip("遺伝的アルゴリズムを担当するGAコンポーネント")]
    public GA ga;

    [Tooltip("個体の描画を担当するDrawingManegerコンポーネント")]
    //public DrawingManeger drawingManager;

    public DrawingManeger[] drawingManagers;

    [Header("UI要素")]
    public GameObject nextGenerationButton;
    public GameObject canvas;
    public GameObject[] objBGs;

    // ===== スクリプト内部で使う変数 =====
    private Individual[] population;
    private int[] fitnessValues;
    public TextMeshProUGUI generationText;

    void Start()
    {
        // --- 必須コンポーネントが設定されているかチェック ---
        if (ga == null)
        {
            Debug.LogError("GAスクリプトがインスペクターで設定されていません！", this.gameObject);
            return;
        }
        if (drawingManagers == null)
        {
            Debug.LogError("DrawingManegerスクリプトがインスペクターで設定されていません！", this.gameObject);
            return;
        }

        // --- 初期化処理 ---
        int popSize = ga.POP_SIZE;
        population = new Individual[popSize];
        fitnessValues = new int[popSize];

        for (int i = 0; i < popSize; ++i)
        {
            population[i] = new Individual();
        }

        // GAに初期集団を渡して、遺伝子をランダムに初期化してもらう
        ga.Initialize(population);

        UpdateGenerationText();

        // --- 最初の個体を描画 ---
        // 最初の集団の0番目の個体を描画
        if (population != null && population.Length > 0)
        {
            Debug.Log("初期集団の最初の個体を描画します。");
            // この命令が正しく動作するようになります
            for (int i = 0; i < 10; i++)
            {
                drawingManagers[i].DrawImageFromGene(population[i].gene);
            }
        }


    }

    /// <summary>
    /// UIのボタンなどから呼び出して、次の世代を生成・描画するメソッド
    /// </summary>
    public void RenderNextGeneration()
    {
        int i = 0;
        int[] score = new int[ga.POP_SIZE];//ga.POP_SIZE:10
        foreach (var dropdown_obj in dropdown_obj_list)
        {

            // ゲームオブジェクトからTMP_Dropdownコンポーネントを取得
            TMP_Dropdown dropdown = dropdown_obj.GetComponent<TMP_Dropdown>();

            score[i] = dropdown.value;

            i++;

        }

        for (int j = 0; j < 10; j++)
        {
            Debug.Log(score[j]);
        }

       population = ga.Evolve(score);



        UpdateGenerationText();

        for (int k = 0; k < population.Length; k++){
            {
                drawingManagers[k].DrawImageFromGene(population[k].gene);
               
            }
        }

        //ドロップダウンの初期化
        foreach (var dropdown_obj in dropdown_obj_list)
        {
            if (dropdown_obj != null)
            {
                TMP_Dropdown dropdown = dropdown_obj.GetComponent<TMP_Dropdown>();
                if (dropdown != null)
                {
                    // valueプロパティに0を設定することで、先頭の選択肢に戻す
                    dropdown.value = 0;
                }
            }
        }

        CheckGenerationLimit();



        // // GAに評価値を渡して、次の世代の個体群を生成してもらう
        //     if (ga != null)
        //     {
        //         population = ga.Evolve(fitnessValues);
        //     }

        // // 新しい世代の最初の個体を描画する
        // if (drawingManagers != null && population != null && population.Length > 0)
        // {
        //     Debug.Log("新しい世代の最初の個体を描画します。");
        //     for (int i = 0; i < 10; i++)
        //     {
        //         drawingManagers[i].DrawImageFromGene(population[i].gene);
        //     }
        // }
    }

    //世代数表示
    private void UpdateGenerationText()
    {
        if (generationText != null && ga != null)
        {
            // GAインスタンスから最新の世代数を取得してUIに表示
            generationText.text = $"{ga.currentGeneration}";
        }
    }

    private void CheckGenerationLimit()
    {
        if (ga != null && nextGenerationButton != null)
        {
            // 現在の世代数が最大世代数以上になったか？
            if (ga.currentGeneration >= ga.MAX_GEN)
            {
                // ボタンをクリックできないようにする
                nextGenerationButton.SetActive(false);
                
            }

        }
    }
}

