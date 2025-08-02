using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using TMPro;

public class MenuController : MonoBehaviour
{
    // Ссылка на кнопку Play - это переменная, которая будет хранить объект кнопки
    // [SerializeField] делает эту переменную видимой в инспекторе Unity
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text versionText;

    // Start вызывается один раз при запуске скрипта
    void Start()
    {   
        YG.YG2.StickyAdActivity(true); // Показать баннер в меню
        // Проверяем, что кнопка назначена
        if (playButton != null)
        {
            // Подключаем метод PlayGame к событию нажатия кнопки
            // Когда игрок нажмет кнопку, автоматически вызовется метод PlayGame
            playButton.onClick.AddListener(PlayGame);
        }
        else
        {
            // Если кнопка не назначена, выводим ошибку в консоль
            Debug.LogError("Кнопка Play не назначена в MenuController!");
        }
        // Выводим версию игры, если назначен текстовый объект
        if (versionText != null)
        {
            versionText.text = "v" + Application.version;
        }
    }

    // Update вызывается каждый кадр (много раз в секунду)
    void Update()
    {
        // Пока оставляем пустым - здесь можно добавить логику, которая должна выполняться постоянно
    }

    // Этот метод будет вызываться при нажатии кнопки Play
    public void PlayGame()
    {
        // Выводим сообщение в консоль для отладки
        Debug.Log("Кнопка Play нажата! Загружаем игровую сцену...");
        
        // Очищаем сохраненный счет для продолжения игры при начале новой игры
        PlayerPrefs.DeleteKey("SavedScoreForContinue");
        PlayerPrefs.Save();
        Debug.Log("Очищен сохраненный счет для продолжения игры");
        
        // Загружаем сцену с именем "GameScene"
        // SceneManager.LoadScene - это встроенная функция Unity для загрузки сцен
        SceneManager.LoadScene("GameScene");
    }
}
