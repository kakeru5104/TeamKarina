using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // UnityEngine.UIも念のため残しておきます
using TMPro; // TextMeshProのドロップダウンに必須

public class CanvasMaster : MonoBehaviour
{
    // インスペクターで設定するドロップダウンのゲームオブジェクトリスト
    public List<GameObject> dropdown_obj_list = new List<GameObject>();
    
    // 取得した値を格納するリスト
    public List<int> value_list;

    // Startは最初のフレームの更新前に呼び出されます
    void Start()
    {
        
    }

    // Updateは毎フレーム呼び出されます
    void Update()
    {
        
    }

    /// <summary>
    /// リストに登録された全てのドロップダウンから値を取得し、リストを更新します。
    /// </summary>
    public void GetValue()
    {
        // 値を格納するリストを初期化
        value_list = new List<int>();

        // 登録された各ドロップダウンオブジェクトに対して処理を実行
        foreach (var dropdown_obj in dropdown_obj_list)
        {
            // ゲームオブジェクトからTMP_Dropdownコンポーネントを取得
            TMP_Dropdown dropdown = dropdown_obj.GetComponent<TMP_Dropdown>();

            if (dropdown != null)
            {
                // 選択されているオプションのテキストを取得
                string selectedText = dropdown.options[dropdown.value].text;
                
                // int.TryParseで数値への変換を試みる
                int parsedValue;
                if (int.TryParse(selectedText, out parsedValue))
                {
                    // 変換に成功した場合、その数値をリストに追加
                    value_list.Add(parsedValue);
                }
                else
                {
                    // 変換に失敗した場合、デフォルト値として「1」を追加
                    value_list.Add(1);
                }

                // 【修正点】値を取得した後、このドロップダウンの選択を最初の項目にリセット
                dropdown.value = 0;
            }
            else
            {
                // オブジェクトにTMP_Dropdownコンポーネントがなかった場合、「1」を追加
                value_list.Add(1);
            }
        }
    }
}