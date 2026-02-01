using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public enum ButtonType
    {
        StartGame,
        MainGame,
        ReturnGame,
        SettingOpen,
        SettingClose
    }

    [Header("UI設定（空でも自動で探します）")]
    public CanvasGroup TitleGroup;
    public GameObject TitleButtonGroup;
    public GameObject CloseButtonGroup;

    [Header("ボタンのタイプを選んでください")]
    public ButtonType type;

    public void OnClick()
    {
        switch (type)
        {
            case ButtonType.StartGame:
            case ButtonType.ReturnGame:
                SceneManager.LoadScene("MainScene");
                break;
            case ButtonType.MainGame:
                SceneManager.LoadScene("EndScene");
                break;
            case ButtonType.SettingOpen:
                //this.OpenSetting();
                break;
            case ButtonType.SettingClose:
                //this.CloseSetting();
                break;
        }
    }

    public void OpenSetting()
    {
        // 中身が空なら、名前で直接探しに行く
        if (TitleGroup == null) TitleGroup = GameObject.Find("TitleGroup")?.GetComponent<CanvasGroup>();
        if (TitleButtonGroup == null) TitleButtonGroup = GameObject.Find("TitleButtonGroup");

        // 中身が見つかった時だけ処理する
        if (TitleGroup != null)
        {
            TitleGroup.interactable = false;
            TitleGroup.blocksRaycasts = false;
            
        }

        if (TitleButtonGroup != null)
        {
            TitleButtonGroup.SetActive(false);
        }
    }

    public void CloseSetting()
    {
        // 閉じる時の処理
        if (TitleGroup != null)
        {
            TitleGroup.interactable = true;
            TitleGroup.blocksRaycasts = true;
        }
        if (TitleButtonGroup != null)
        {
            TitleButtonGroup.SetActive(true);
        }
        
    }
}