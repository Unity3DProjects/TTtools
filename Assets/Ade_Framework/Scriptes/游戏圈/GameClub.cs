using Ade_Framework;
using UnityEngine;
using UnityEngine.UI;

public class GameClub : MonoBehaviour
{
    void Start()
    {
#if Ade_WX
        Button button = GetComponent<Button>();
        if (button == null)
        {
            gameObject.SetActive(false);
            return;
        }

        button.onClick.AddListener(OnClick);
#else
        gameObject.SetActive(false);
#endif
    }

    public void OnClick()
    {
#if Ade_WX
        ADManager.Instance.ShowGameClub();
#else
        gameObject.SetActive(false);
#endif
    }
}
