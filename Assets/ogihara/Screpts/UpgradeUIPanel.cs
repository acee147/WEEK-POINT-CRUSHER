using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// UpgradeManagerで定義された構造体と同じものを再定義または参照します。
// ここでは、UpgradeManagerのUpgradeOptionをpublicにし、このスクリプトで参照するようにします。

public class UpgradeUIPanel : MonoBehaviour
{
    // UpgradeManager.UpgradeOption と同じ構造体の定義
    // (UpgradeManagerの構造体をpublicにすれば、この定義は不要です。ここでは独立のため再定義)
    [System.Serializable]
    public struct UpgradeOption
    {
        public string type;
        public int weight;
    }

    [Header("UI要素の参照")]
    [Tooltip("ヒエラルキーの順にボタンを割り当ててください。")]
    [SerializeField]
    private Button[] choiceButtons = new Button[3]; // 3つのボタンを配列で管理

    [Tooltip("ボタンの子にあるテキストコンポーネントを割り当ててください。")]
    [SerializeField]
    private TextMeshProUGUI[] buttonTexts = new TextMeshProUGUI[3]; // 3つのボタンのテキスト

    private UpgradeManager upgradeManager; // UpgradeManagerへの参照

    void Start()
    {
        // UpgradeManagerのインスタンスを取得
        upgradeManager = UpgradeManager.Instance;
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeManagerが見つかりません！");
            return;
        }

        // ゲーム開始時にボタンのOnClickイベントを登録 (一度だけ行えば良い)
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            // クロージャのためにローカル変数にコピー
            int index = i;

            // 既存のリスナーを全てクリアしてから追加 (エディタからの設定と競合しないように)
            choiceButtons[i].onClick.RemoveAllListeners();

            // ボタンが押されたら、OnChoiceSelected(index) を呼び出すように設定
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
        }
    }

    // UpgradeManagerのShowUpgradeChoice()から呼ばれ、抽選結果をUIに反映させる
    public void SetChoices(List<UpgradeManager.UpgradeOption> choices)
    {
        if (choices.Count != choiceButtons.Length)
        {
            Debug.LogError("抽選された選択肢の数とボタンの数が一致しません。");
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            // ボタンのテキストを、ユーザー向けの表示名に設定
            buttonTexts[i].text = GetDisplayName(choices[i].type);
            choiceButtons[i].gameObject.SetActive(true); // ボタンを確実に表示
        }
    }

    // ボタンが押されたときに呼ばれるリスナーメソッド
    private void OnChoiceSelected(int index)
    {
        // UpgradeManagerが持つ抽選結果リストを参照し、選択されたインデックスのアップグレードタイプを取得
        if (upgradeManager != null && upgradeManager.CurrentChoices != null && index < upgradeManager.CurrentChoices.Count)
        {
            // 押されたボタンに対応するアップグレードの'type'文字列を取得
            string selectedUpgradeType = upgradeManager.CurrentChoices[index].type;

            // UpgradeManagerの適用メソッドを呼び出し、ゲームを再開
            upgradeManager.ApplyUpgrade(selectedUpgradeType);
        }
        else
        {
            Debug.LogError($"選択されたインデックス ({index}) が無効か、UpgradeManagerのリストが空です。");
        }
    }

    // アップグレードの内部名（"FireRate"など）をユーザー向けの表示名に変換するヘルパーメソッド
    private string GetDisplayName(string type)
    {
        switch (type)
        {
            case "FireRate": return "れんしゃそくど";
            case "MoveSpeed": return "スピード";
            case "SlowMotionUses": return "スローモーション";
            case "WideShot": return "ワイドショット ";
            // 未知のアップグレードが抽選された場合のエラー処理
            default: return $"エラー: {type}";
        }
    }
}