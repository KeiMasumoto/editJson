using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// 配列の要素に使用するクラス
[Serializable]
public class Sample
{
    public string day;
    public string open;
    public string close;
}

public class EditJson : MonoBehaviour
{
    private string dataPath;

    void Awake()
    {
        dataPath = Application.streamingAssetsPath + "/day_data.json";
    }
    // Start is called before the first frame update
    void Start()
    {
        // JSON配列の読み取り
        string json = File.ReadAllText(dataPath);

        // JSON配列 → C#配列への変換
        Sample[] array = JsonHelper.FromJson<Sample>(json);

        // // C#配列 → JSON配列への変換
        // string json2 = JsonHelper.ToJson(array);

        // // ★C#のリスト → JSON配列への変換
        // var list = new List<Sample>(array);
        // string json3 = JsonHelper.ToJson(list);

        // // ★Linqの結果 → JSON配列への変換
        // var items = list.Where(s => s.id > 200);
        // string json4 = JsonHelper.ToJson(items);
        // Sample data = JsonUtility.FromJson<Sample>(array);
        Debug.Log(array[0].day);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// <see cref="JsonUtility"/> に不足している機能を提供します。
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 指定した string を Root オブジェクトを持たない JSON 配列と仮定してデシリアライズします。
        /// </summary>
        public static T[] FromJson<T>(string json)
        {
            // ルート要素があれば変換できるので
            // 入力されたJSONに対して(★)の行を追加する
            //
            // e.g.
            // ★ {
            // ★     "array":
            //        [
            //            ...
            //        ]
            // ★ }
            //
            string dummy_json = $"{{\"{DummyNode<T>.ROOT_NAME}\": {json}}}";

            // ダミーのルートにデシリアライズしてから中身の配列を返す
            var obj = JsonUtility.FromJson<DummyNode<T>>(dummy_json);
            return obj.array;
        }

        /// <summary>
        /// 指定した配列やリストなどのコレクションを Root オブジェクトを持たない JSON 配列に変換します。
        /// </summary>
        /// <remarks>
        /// 'prettyPrint' には非対応。整形したかったら別途変換して。
        /// </remarks>
        public static string ToJson<T>(List<T> collection)
        {
            string json = JsonUtility.ToJson(new DummyNode<T>(collection)); // ダミールートごとシリアル化する
            int start = DummyNode<T>.ROOT_NAME.Length + 4;
            int len = json.Length - start - 1;
            return json.Substring(start, len); // 追加ルートの文字を取り除いて返す
        }

        // 内部で使用するダミーのルート要素
        [Serializable]
        private struct DummyNode<T>
        {
            // 補足:
            // 処理中に一時使用する非公開クラスのため多少設計が変でも気にしない

            // JSONに付与するダミールートの名称
            public const string ROOT_NAME = nameof(array);
            // 疑似的な子要素
            public T[] array;
            // コレクション要素を指定してオブジェクトを作成する
            public DummyNode(List<T> collection) => this.array = collection.ToArray();
        }
    }
}
