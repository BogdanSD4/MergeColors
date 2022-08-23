using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMenuControl : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private List<RectTransform> _menuPunkt;

    public AllUpdate allUpdate = new AllUpdate();
    public const float MenuDistance = 220;

    private void Start()
    {
        allUpdate.storage.SetPrice();
        allUpdate.paintSeller.SetPrice();
        allUpdate.clicker.SetPrice();
    }

    public void ChangeContentSize(float value, int menu)
    {
        _content.sizeDelta += new Vector2(0, value);

        for (int i = 0; i < _menuPunkt.Count; i++)
        {
            if (i >= menu)
            {
                _menuPunkt[i].transform.localPosition -= new Vector3(0, value);
            }
        }
    }

    public int[] GetOpenCellsValue()
    {
        int[] array = new int[100];
        for (int i = 0; i < allUpdate.labMenu.labPrice.Count; i++)
        {
            array[i] = allUpdate.labMenu.labPrice[i].Menu.NewColorControl.MenuMerge.OpenCell;
        }
        return array;
    }
}
[System.Serializable]
public class AllUpdate
{
    public LabMenu labMenu = new LabMenu();
    public ClassUpdate classUpdate = new ClassUpdate();
    public Storage storage = new Storage();
    public MergeUpdate mergeUpdate = new MergeUpdate();
    public SellCoin paintSeller = new SellCoin();
    public ClickerMenu clicker = new ClickerMenu();


    [System.Serializable]
    public class LabMenu
    {
        public List<LabPrice> labPrice;

        public (int, double) LabUpdate(int numMenu, int value)
        {
            int num = labPrice[numMenu - 1].Menu.UpdateMenu(value);
            double num0 = labPrice[numMenu - 1].PriceCount();
            return (num, num0);
        }
        public bool GetLabMenuActive(int numMenu) => labPrice[numMenu - 1].Menu.NewColorControl.MenuMerge.Active; 

        [System.Serializable]
        public class LabPrice
        {
            public LabMenuControl Menu;
            public double Price;
            public double FactorUpdate;

            public double StartPriceCount()
            {
                int count = Menu.NewColorControl.MenuMerge.OpenCell;
                for (int i = 1; i < count; i++)
                {
                    Price *= FactorUpdate;
                    FactorUpdate += (FactorUpdate * 0.3f);
                }
                return Price;
            }

            public double PriceCount()
            {
                Price *= FactorUpdate;
                FactorUpdate += (FactorUpdate * 0.3f);
                return Price;
            }
        }
    }
    [System.Serializable]
    public class ClassUpdate
    {

    }
    [System.Serializable]
    public class Storage
    {
        [SerializeField] private TimeSystem _timeSystem;
        public double UpdatePriceCoin;
        public double UpdatePricePaint;

        public double Factor;

        public double UpCoin()
        {
            _timeSystem.LevelUpCoin();
            UpdatePriceCoin *= Factor;
            return UpdatePriceCoin;
        }
        public double UpPaint()
        {
            _timeSystem.LevelUpPaint();
            UpdatePricePaint *= Factor;
            return UpdatePricePaint;
        }


        public void SetPrice()
        {
            int coin = _timeSystem.CoinLevel;
            int paint = _timeSystem.PaintLevel;

            for(int i = 1; i < coin; i++)
            {
                UpdatePriceCoin *= Factor;
            }

            for (int i = 1; i < paint; i++)
            {
                UpdatePricePaint *= Factor;
            }
        }
    }
    [System.Serializable]
    public class MergeUpdate
    {

    }
    [System.Serializable]
    public class SellCoin
    {
        [SerializeField] private GAMEControler _gameControler;
        public double UpdatePrice;
        public double UpdateFactor;
        private int level;

        public double UpdateSeller()
        {
            _gameControler.SellerUpdate();
            level++;
            UpdatePrice *= UpdateFactor;
            PlayerPrefs.SetInt("SellerUpdateLevel", level);
            return UpdatePrice;
        }

        public void SetPrice()
        {
            if ((level = PlayerPrefs.GetInt("SellerUpdateLevel")) == 0) level = 1;
            for(int i = 1; i < level; i++)
            {
                UpdatePrice *= UpdateFactor;
            }
        }
    }
    [System.Serializable]
    public class ClickerMenu
    {
        [SerializeField] private Clicker _clicker;
        public double PriceGetBuyClick;
        public float FactorClick;
        private int LevelClick;

        public double PriceMaxFactor;
        public float FactorFactor;
        private int LevelFactor; 

        public double PriceTapAmount;
        public float FactorAmount;
        private int LevelAmount;


        public double UpClick()
        {
            _clicker.UpdateGetByClick();
            PriceGetBuyClick *= FactorClick;
            LevelClick++;
            PlayerPrefs.SetInt("LevelClick", LevelClick);
            return PriceGetBuyClick;
        }
        public double UpFactor()
        {
            _clicker.UpdateMaxFactor();
            PriceMaxFactor *= FactorFactor;
            LevelFactor++;
            PlayerPrefs.SetInt("LevelFactor", LevelFactor);
            return PriceMaxFactor;
        }
        public double UpAmount()
        {
            _clicker.MultiplierUpdate();
            PriceTapAmount *= FactorAmount;
            LevelAmount++;
            PlayerPrefs.SetInt("LevelAmount", LevelAmount);
            return PriceTapAmount;
        }


        public void SetPrice()
        {
            if ((LevelClick = PlayerPrefs.GetInt("LevelClick")) == 0) LevelClick = 1;
            if ((LevelFactor = PlayerPrefs.GetInt("LevelFactor")) == 0) LevelFactor = 1;
            if ((LevelAmount = PlayerPrefs.GetInt("LevelAmount")) == 0) LevelAmount = 1;

            for (int i = 1; i < LevelClick; i++) PriceGetBuyClick *= FactorClick;
            for (int i = 1; i < LevelFactor; i++) PriceMaxFactor *= FactorFactor;
            for (int i = 1; i < LevelAmount; i++) PriceTapAmount *= FactorAmount;

            _clicker.SetStartPreferance(LevelClick, LevelFactor, LevelAmount);
        }
    }
    
}
