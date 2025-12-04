using System;
using UnityEngine;
using UnityEngine.UI;

public class MeteoUI : MonoBehaviour
{
    [SerializeField] private Button rainButton;
    [SerializeField] private Button sunButton;
    private Selector selector;
    private void Awake()
    {
        selector = GetComponentInChildren<Selector>();
    }

    private void Start()
    {
        MeteoManager.Instance.weatherChanged += isRaining => { selector.Select(isRaining ? rainButton : sunButton); };
        //selector.Select(sunButton);

        rainButton.onClick.AddListener(() =>
        {
            MeteoManager.Instance.SetWeather(MeteoState.Raining);
        });
        
        sunButton.onClick.AddListener(() =>
        {
            MeteoManager.Instance.SetWeather(MeteoState.Sunny);
        });
        
        MeteoManager.Instance.weatherChanged += isRaining =>
        {
            selector.Select(isRaining ? rainButton : sunButton);
        };
    }
}
