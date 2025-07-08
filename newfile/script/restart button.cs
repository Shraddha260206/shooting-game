using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class RestartTextClickable : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI restartText;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Start()
    {
#if UNITY_ANDROID
        restartText.text = "Tap to restart";
#else
        restartText.text = "Press R to restart";
#endif
    }
}
