using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] private BetterButton finishButton;

    private void Awake()
    {
        finishButton.onClick.AddListener(OnPressFinish);
    }

    private void OnPressFinish()
    {
        Globals<RunManager>.Instance.LoadScene(SceneType.Map);
    }
}
