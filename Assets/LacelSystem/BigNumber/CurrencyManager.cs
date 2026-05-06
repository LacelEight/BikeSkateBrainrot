using UnityEngine;
using TMPro; // Sử dụng TextMeshPro để hiển thị đẹp hơn

namespace LacelSDK
{
    public class CurrencyManager : MonoBehaviour
    {
        public TextMeshProUGUI goldText;
        [SerializeField] private double currentGold = 0;

        void Start()
        {
            AddGold(1500); // Thử nghiệm cộng 1.5k
            UpdateUI();
        }

        // Hàm cộng tiền
        public void AddGold(double amount)
        {
            if (amount < 0) return;
            currentGold += amount;
            UpdateUI();
        }

        // Hàm trừ tiền (Trả về true nếu đủ tiền để trừ)
        public bool SubtractGold(double amount)
        {
            if (amount < 0) return false;

            if (currentGold >= amount)
            {
                currentGold -= amount;
                UpdateUI();
                return true;
            }

            Debug.Log("Không đủ tiền!");
            return false;
        }

        // Hàm cập nhật hiển thị lên UI
        private void UpdateUI()
        {
            if (goldText != null)
            {
                goldText.text = "Gold: " + BigNumberFormatter.Format(currentGold);
            }
        }

        // Ví dụ: Nhấn nút để test trong Unity
        [ContextMenu("Test Add 1M")]
        public void TestAdd() => AddGold(1000000);

        [ContextMenu("Test Subtract 500k")]
        public void TestSub() => SubtractGold(500000);
    }
}