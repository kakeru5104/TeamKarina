using UnityEngine;
using System.Collections.Generic; // Listはもう使いませんが、念のため残しています
using System; // ArgumentExceptionのために追加

public class convert : MonoBehaviour
{
    // ===== publicなプロパティ =====
    // 計算結果は他のスクリプトから読み取れるようにpublicにしておきます。
    public int[] Rectangle_XY { get; private set; } = new int[4];
    public int[] Rectangle_Color { get; private set; } = new int[3];
    public int[] Ellipse_XY { get; private set; } = new int[4];
    public int[] Ellipse_Color { get; private set; } = new int[3];
    public int[] Background { get; private set; } = new int[3];

    // 遺伝子情報の期待される長さ（GA.csのBIT_COUNTと一致させる）
    private const int EXPECTED_GENE_LENGTH = 59;

    /// <summary>
    /// 外部から渡された遺伝子配列に基づいて、描画用のデータを生成します。
    /// </summary>
    /// <param name="gene">GAによって生成された一個体分の遺伝子配列 (int[])</param>
    public void GenerateDrawingData(int[] gene)
    {
        // --- 引数のチェック ---
        if (gene == null || gene.Length != EXPECTED_GENE_LENGTH)
        {
            Debug.LogError($"引数で渡された遺伝子の長さが不正です。期待値: {EXPECTED_GENE_LENGTH}, 実際の値: {(gene?.Length ?? 0)}");
            return; // エラーが発生した場合は処理を中断
        }
        
        // --- 変換処理 ---
        // 遺伝子配列の読み取り位置を管理するカーソル
        int cursor = 0;

        // 矩形の座標を変換 (4bit * 4項目 = 16bit)
        for (int j = 0; j < 4; j++)
        {
            Rectangle_XY[j] = ConvertToDecimal(cursor, 4, gene) * 4;
            cursor += 4;
        }

        // 矩形の色を変換 (3bit * 3項目 = 9bit)
        for (int j = 0; j < 3; j++)
        {
            Rectangle_Color[j] = ConvertToDecimal(cursor, 3, gene) * 36;
            cursor += 3;
        }

        // 楕円の座標を変換 (4bit * 4項目 = 16bit)
        for (int j = 0; j < 4; j++)
        {
            Ellipse_XY[j] = ConvertToDecimal(cursor, 4, gene) * 6;
            cursor += 4;
        }

        // 楕円の色を変換 (3bit * 3項目 = 9bit)
        for (int j = 0; j < 3; j++)
        {
            Ellipse_Color[j] = ConvertToDecimal(cursor, 3, gene) * 36;
            cursor += 3;
        }

        // 背景色を変換 (3bit * 3項目 = 9bit)
        for (int j = 0; j < 3; j++)
        {
            Background[j] = ConvertToDecimal(cursor, 3, gene) * 36;
            cursor += 3;
        }
        
        // Debug.Log("遺伝子データから描画情報への変換が完了しました。");
    }

    /// <summary>
    /// 遺伝子配列の指定された部分を10進数に変換します。
    /// </summary>
    /// <param name="startIndex">読み取り開始インデックス</param>
    /// <param name="length">読み取るビット数</param>
    /// <param name="gene">遺伝子配列</param>
    /// <returns>変換後の10進数の値</returns>
    private int ConvertToDecimal(int startIndex, int length, int[] gene)
    {
        int value = 0;
        int powerOfTwo = 1;
        
        // 配列の末尾から計算する（リトルエンディアン方式）
        for (int i = 0; i < length; i++)
        {
            int index = startIndex + length - 1 - i;
            if (index < gene.Length && gene[index] == 1)
            {
                value += powerOfTwo;
            }
            powerOfTwo *= 2;
        }
        return value;
    }
}
