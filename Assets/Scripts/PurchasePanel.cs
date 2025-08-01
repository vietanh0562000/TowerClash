using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class PurchasePanel : MonoBehaviour
    {
        public GameObject shopPanel;
        public TextMeshProUGUI coinText;
        public void CloseShop()
        {
            shopPanel.SetActive(false);
        }
        
        public void OpenShop()
        {
            shopPanel.SetActive(true);
            UpdateText();
        }

        public void UpdateText()
        {
            coinText.text = PlayerPrefs.GetInt("Coins").ToString();
        }

        private void Update()
        {
            UpdateText();
        }
        
        public void PauseGame()
        {
            Time.timeScale = 0;
            var coin = PlayerPrefs.GetInt("Coins");
            if (coin > 100)
            {
                PlayerPrefs.SetInt("Coins",coin - 100);
                StartCoroutine(TimeoutExample());
            }
            else
            {
                OpenShop();
            }
        }
        
        IEnumerator TimeoutExample()
        {
            Debug.Log("Start timeout");
            yield return new WaitForSecondsRealtime(2f);
            Debug.Log("End timeout");
            Time.timeScale = 1;
        }
    }
}