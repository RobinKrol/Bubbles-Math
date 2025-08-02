using UnityEngine;
using YG;
using UnityEngine.SceneManagement;
public class LeaderboardManager : MonoBehaviour
{


    public void GoToMenu()
    {
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("MenuScene");
        
    }


}
