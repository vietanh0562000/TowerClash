using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasingManager : MonoBehaviour
{
   public void OnPressDown(int i)
   {
      var coins = PlayerPrefs.GetInt("Coins");
      switch (i)
      {
         case 1:
             PlayerPrefs.SetInt("Coins", coins + 100);  
             IAPManager.Instance.BuyProductID(IAPKey.PACK1);
            break;
         case 2:
            PlayerPrefs.SetInt("Coins", coins + 200); 
            IAPManager.Instance.BuyProductID(IAPKey.PACK2);
            break;
         case 3:
            PlayerPrefs.SetInt("Coins", coins + 500); 
            IAPManager.Instance.BuyProductID(IAPKey.PACK3);
            break;
         case 4:
            PlayerPrefs.SetInt("Coins", coins + 1000); 
            IAPManager.Instance.BuyProductID(IAPKey.PACK4);
            break;
      }
   }

   public void Sub(int i)
   {
      GameDataManager.Instance.playerData.SubDiamond(i);
   }
}
