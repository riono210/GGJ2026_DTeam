using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{

    public enum ButtonType//リスト
    {
        StartGame,//メインシーンへ
        MainGame,//エンドシーンへ
        ReturnGame,//メインシーンへ
    }

    [Header("ボタンのタイプを選んでください")]
    public ButtonType type;


    public void OnClick()
    {
        switch (type)
        {
            case ButtonType.StartGame:
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
                break;
            case ButtonType.MainGame:
                UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
                break;
            case ButtonType.ReturnGame:
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
