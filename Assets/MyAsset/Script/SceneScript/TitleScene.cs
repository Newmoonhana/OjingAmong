using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public void InputGameStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
