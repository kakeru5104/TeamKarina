using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrawingManeger : MonoBehaviour
{
    [SerializeField] private RawImage drawingCanvas;
    [SerializeField] private convert convertScript;

    //===== 内部で使用する変数 =====
    private int ellipse_x1, ellipse_x2, ellipse_y1, ellipse_y2;
    private int rectangle_x1, rectangle_x2, rectangle_y1, rectangle_y2;
    private Color backgroundColor, retangleColor, elipseColor;

    //===== キャンバスサイズ =====
    private const int CANVAS_HEIGHT = 120;
    private const int CANVAS_WIDTH = 120;

    /// <summary>
    /// このメソッドは空にしておきます。
    /// 描画は外部からの指示(DrawImageFromGene)によってのみ実行されます。
    /// </summary>
    void Start()
    {
        // Start時に自動で描画しないように、ここの処理は空にします。
    }

    /// <summary>
    /// 外部から遺伝子データを受け取り、それを元にイメージを描画する公開メソッド。
    /// </summary>
    /// <param name="gene">GAによって生成された一個体分の遺伝子配列 (int[])</param>
    public void DrawImageFromGene(int[] gene)
    {
        if (convertScript == null)
        {
            Debug.LogError("convertスクリプトがインスペクターで設定されていません！", this.gameObject);
            return;
        }
        if (drawingCanvas == null)
        {
            Debug.LogError("drawingCanvasがインスペクターで設定されていません！", this.gameObject);
            return;
        }

        // 1. convertスクリプトに遺伝子を渡して、10進数に変換してもらう
        convertScript.GenerateDrawingData(gene);

        // 2. 変換されたデータを内部変数に格納する
        StoreConvertedData();

        // 3. 格納したデータを使って、実際に描画処理を実行する
        Draw();
    }
    
    /// <summary>
    /// convertスクリプトから変換後のデータを取得し、クラス内の変数に格納します。
    /// </summary>
    private void StoreConvertedData()
    {
        //--- 矩形の座標を取得 ---
        rectangle_x1 = convertScript.Rectangle_XY[0];
        rectangle_y1 = convertScript.Rectangle_XY[1];
        rectangle_x2 = convertScript.Rectangle_XY[2];
        rectangle_y2 = convertScript.Rectangle_XY[3];

        //--- 楕円の座標を取得 ---
        ellipse_x1 = convertScript.Ellipse_XY[0];
        ellipse_y1 = convertScript.Ellipse_XY[1];
        ellipse_x2 = convertScript.Ellipse_XY[2];
        ellipse_y2 = convertScript.Ellipse_XY[3];

        //--- 各種色情報を取得 (int[R,G,B] から Color32 に変換) ---
        backgroundColor = new Color32(
            (byte)convertScript.Background[0], 
            (byte)convertScript.Background[1], 
            (byte)convertScript.Background[2], 
            255);
            
        retangleColor = new Color32(
            (byte)convertScript.Rectangle_Color[0], 
            (byte)convertScript.Rectangle_Color[1], 
            (byte)convertScript.Rectangle_Color[2], 
            255);

        elipseColor = new Color32(
            (byte)convertScript.Ellipse_Color[0], 
            (byte)convertScript.Ellipse_Color[1], 
            (byte)convertScript.Ellipse_Color[2], 
            255);
    }


    /// <summary>
    /// 内部変数に格納されたデータに基づいて、キャンバスに図形を描画します。
    /// </summary>
    private void Draw()
    {
        Texture2D canvasTexture = new Texture2D(CANVAS_WIDTH, CANVAS_HEIGHT);
        drawingCanvas.texture = canvasTexture;

        // 背景処理
        for (int y = 0; y < CANVAS_HEIGHT; y++)
        {
            for (int x = 0; x < CANVAS_WIDTH; x++)
            {
                canvasTexture.SetPixel(x, y, backgroundColor);
            }
        }

        // 図形描画
        DrawRectangle(canvasTexture, rectangle_x1, rectangle_y1, rectangle_x2, rectangle_y2, retangleColor);
        DrawRectangle(canvasTexture, rectangle_y1, rectangle_x1, rectangle_y2, rectangle_x2, retangleColor);
        DrawEllipse(canvasTexture, ellipse_x1, ellipse_y1, ellipse_x2, ellipse_y2, elipseColor);
        DrawEllipse(canvasTexture, ellipse_y1, ellipse_x1, ellipse_y2, ellipse_x2, elipseColor);

        // 反転処理
        RectInt sourceRect = new RectInt(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT / 2);
        Vector2Int destinationPosition = new Vector2Int(0, CANVAS_HEIGHT / 2);
        FlipAndDrawRegionVertically(canvasTexture, sourceRect, destinationPosition);
        
        sourceRect = new RectInt(0, 0, CANVAS_WIDTH / 2, CANVAS_HEIGHT);
        destinationPosition = new Vector2Int(CANVAS_WIDTH / 2, 0);
        FlipAndDrawRegionHorizontally(canvasTexture, sourceRect, destinationPosition);

        canvasTexture.Apply();
    }

    #region Drawing Helper Methods
    //=====上下反転関数=====
    private void FlipAndDrawRegionVertically(Texture2D texture, RectInt sourceRect, Vector2Int destinationTopLeft)
    {
        Color[] sourcePixels = texture.GetPixels(sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height);
        Color[] flippedPixels = new Color[sourcePixels.Length];

        for (int y = 0; y < sourceRect.height; y++)
        {
            for (int x = 0; x < sourceRect.width; x++)
            {
                int sourceIndex = y * sourceRect.width + x;
                int flippedY = (sourceRect.height - 1) - y;
                int flippedIndex = flippedY * sourceRect.width + x;
                flippedPixels[flippedIndex] = sourcePixels[sourceIndex];
            }
        }
        texture.SetPixels(destinationTopLeft.x, destinationTopLeft.y, sourceRect.width, sourceRect.height, flippedPixels);
    }

    //=====左右反転関数=====
    private void FlipAndDrawRegionHorizontally(Texture2D texture, RectInt sourceRect, Vector2Int destinationTopLeft)
    {
        Color[] sourcePixels = texture.GetPixels(sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height);
        Color[] flippedPixels = new Color[sourcePixels.Length];

        for (int y = 0; y < sourceRect.height; y++)
        {
            for (int x = 0; x < sourceRect.width; x++)
            {
                int sourceIndex = y * sourceRect.width + x;
                int flippedX = (sourceRect.width - 1) - x;
                int flippedIndex = y * sourceRect.width + flippedX;
                flippedPixels[flippedIndex] = sourcePixels[sourceIndex];
            }
        }
        texture.SetPixels(destinationTopLeft.x, destinationTopLeft.y, sourceRect.width, sourceRect.height, flippedPixels);
    }

    //矩形描画関数
    private void DrawRectangle(Texture2D texture, int x1, int y1, int x2, int y2, Color color)
    {
        int minX = Mathf.Min(x1, x2);
        int maxX = Mathf.Max(x1, x2);
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

     //楕円描画関数
    private void DrawEllipse(Texture2D texture, int x1, int y1, int x2, int y2, Color color)
    {
        int minX = Mathf.Min(x1, x2);
        int maxX = Mathf.Max(x1, x2);
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        float centerX = (minX + maxX) / 2.0f;
        float centerY = (minY + maxY) / 2.0f;
        float radiusX = (maxX - minX) / 2.0f;
        float radiusY = (maxY - minY) / 2.0f;

        if (radiusX <= 0 || radiusY <= 0) return;

        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                float term1 = (x - centerX) / radiusX;
                float term2 = (y - centerY) / radiusY;

                if (term1 * term1 + term2 * term2 <= 1)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
    #endregion
}