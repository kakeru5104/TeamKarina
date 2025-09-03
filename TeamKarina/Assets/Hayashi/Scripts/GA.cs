using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GA : MonoBehaviour
{
    // --- GAの定数 ---
    // App.csから参照できるようにpublicに変更
    public readonly int BIT_COUNT = 59;
    public readonly int POP_SIZE = 10;
    public readonly int MAX_GEN = 20;

    private System.Random _rand = new System.Random();
    
    // Individualクラスの集団を保持するように変更
    private List<Individual> _population = new List<Individual>();

    // --- 世代数をUnityに表示するための変数 ---
    [Header("Current GA Status")]
    public int currentGeneration = 0;


    // ================================================================
    // App.csから呼び出される公開メソッド
    // ================================================================

    /// <summary>
    /// App.csから渡された初期集団でGAを初期化します。
    /// 各個体の遺伝子はランダムな値で初期化されます。
    /// </summary>
    public void Initialize(Individual[] initialPopulation)
    {
        if (initialPopulation.Length != POP_SIZE)
        {
            Debug.LogError($"初期集団のサイズが異なります。期待値: {POP_SIZE}, 実際の値: {initialPopulation.Length}");
            return;
        }

        // 渡された配列で内部の集団リストを初期化
        _population = new List<Individual>(initialPopulation);
        
        // 各個体の遺伝子をランダムなビット列で初期化
        foreach(var individual in _population)
        {
            if (individual.gene == null || individual.gene.Length != BIT_COUNT)
            {
                individual.gene = new int[BIT_COUNT];
            }
            InitializeGene(individual.gene);
        }
        
        Debug.Log("GAが初期化され、最初の遺伝子集団が生成されました。");
        LogPopulation("初期集団", _population);
    }
    
    /// <summary>
    /// 次の世代を生成し、新しい個体群を返します。
    /// </summary>
    /// <param name="fitnessValues">現世代の各個体の評価値</param>
    /// <returns>新しい世代の個体群</returns>
    public Individual[] Evolve(int[] fitnessValues)
    {
        currentGeneration++;
        Debug.Log($"\n--- 世代 {currentGeneration} の進化を開始 ---");
        LogPopulation("進化前の集団", _population, fitnessValues);

        List<Individual> nextGeneration = new List<Individual>();

        for (int i = 0; i < POP_SIZE; i++)
        {
            // 親を選択
            int p1Idx = SelectParentIndex(fitnessValues);
            int p2Idx = SelectParentIndex(fitnessValues);


            int retryCount = 0; // 再試行カウンター

            while (p1Idx == p2Idx && retryCount < POP_SIZE)
            {
                p2Idx = SelectParentIndex(fitnessValues);
                retryCount++;
            }

            // もし最大回数試行しても同じ親だった場合、ランダムに選び直す
            if (p1Idx == p2Idx)
            {
                while (p1Idx == p2Idx)
                {
                    p2Idx = (p1Idx + _rand.Next(1, 11)) % POP_SIZE; // とりあえず隣の個体を選ぶなど
                }
                
                Debug.LogWarning($"親の再選択に失敗したため、強制的に異なる親({p2Idx})を選択しました。");
            }
            // 交叉
            int[] childGene = Crossover(
                _population[p1Idx].gene, 
                _population[p2Idx].gene
            );
            
            // 新しい個体を作成
            Individual child = new Individual { gene = childGene };
            nextGeneration.Add(child);
        }
        
        // 突然変異
        if (nextGeneration.Count > 0)
        {
            int mutateIdx = _rand.Next(0, nextGeneration.Count);
            Mutate(nextGeneration[mutateIdx].gene);
            Debug.Log($"個体 {mutateIdx} に突然変異を適用しました。");
        }
        
        // 世代交代
        _population = nextGeneration;
        
        LogPopulation($"世代 {currentGeneration} の新しい集団", _population);
        
        return _population.ToArray();
    }


    // ================================================================
    // 遺伝的アルゴリズムのコアロジック (private)
    // ================================================================

    // 遺伝子をランダムな0か1で初期化する
    private void InitializeGene(int[] gene)
    {
        for (int i = 0; i < gene.Length; i++)
        {
            gene[i] = _rand.Next(0, 2);
        }
    }

    // 評価値に基づいて親のインデックスを選択する（ルーレット選択）
    private int SelectParentIndex(int[] fitnessValues)
    {
        int totalFitness = fitnessValues.Sum();
        if (totalFitness <= 0) return _rand.Next(0, POP_SIZE); // 評価値が全て0以下の場合はランダム

        int roulettePoint = _rand.Next(0, totalFitness);
        int currentSum = 0;
        for (int i = 0; i < POP_SIZE; i++)
        {
            currentSum += fitnessValues[i];
            if (roulettePoint < currentSum)
            {
                return i;
            }
        }
        return POP_SIZE - 1; // 念のため
    }

    // 2つの親の遺伝子から子の遺伝子を生成する（交叉）
    private int[] Crossover(int[] p1Gene, int[] p2Gene)
    {
        int[] childGene = new int[BIT_COUNT];
        for (int i = 0; i < BIT_COUNT; i++)
        {
            // 奇数番目は親1から、偶数番目は親2から遺伝
            childGene[i] = (i % 2 == 1) ? p1Gene[i] : p2Gene[i];
        }
        return childGene;
    }

    // 個体の遺伝子をランダムに反転させる（突然変異）
    private void Mutate(int[] gene)
    {
        Debug.Log("突然変異発生！ 遺伝子列をランダムに再生成します。");
        // 遺伝子配列の全てのビットをランダムに再設定する（InitializeGeneと同じ処理）
        for (int i = 0; i < gene.Length; i++)
        {
            gene[i] = _rand.Next(0, 2);
        }
    }
    // 集団の情報をコンソールに出力するヘルパー関数
    private void LogPopulation(string label, List<Individual> pop, int[] fitness = null)
    {
        Debug.Log($"--- {label} ---");
        for (int i = 0; i < pop.Count; i++)
        {
            string fitnessStr = (fitness != null && i < fitness.Length) ? $" (評価値:{fitness[i]})" : "";
            Debug.Log($"  個体 {i}{fitnessStr}: [{string.Join("", pop[i].gene)}]");
        }
    }
}
