using UnityEngine;

public class DebugTester : MonoBehaviour
{
    [Header("テスト対象の描画マネージャー")]
    [Tooltip("ヒエラルキーから10個のうちの1つをここに設定してください")]
    public DrawingManeger targetManager;

    // 描画結果が分かりやすいように、意図的に作ったテスト用の遺伝子データ
    private int[] testGene = new int[]
    {
        // 矩形の座標 (16bit)
        1,0,0,0,  0,1,0,0,  1,1,0,0,  1,0,1,0,
        // 矩形の色 (9bit) - 赤色っぽくなるはず
        1,1,0,  1,0,1,  0,1,1,
        // 楕円の座標 (16bit)
        0,0,1,0,  1,0,0,1,  0,0,1,1,  1,1,1,0,
        // 楕円の色 (9bit) - 青色っぽくなるはず
        1,0,0,  0,1,0,  1,1,1,
        // 背景の色 (9bit) - 暗い灰色になるはず
        0,0,1,  0,0,1,  0,0,1
    };

    void Start()
    {
        if (targetManager == null)
        {
            Debug.LogError("テスト対象のDrawingManagerが設定されていません！", this.gameObject);
            return;
        }

        Debug.Log("--- デバッグテスト開始 ---");
        Debug.Log($"テスト対象: {targetManager.gameObject.name}");
        Debug.Log($"テスト用遺伝子: [{string.Join("", testGene)}]");
        
        // テスト用の遺伝子データを渡して、描画を指示
        targetManager.DrawImageFromGene(testGene);

        Debug.Log("--- デバッグテスト完了 ---");
    }
}
