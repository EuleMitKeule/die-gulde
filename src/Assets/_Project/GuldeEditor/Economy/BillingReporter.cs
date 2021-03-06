using GuldeLib;
using GuldeLib.Economy;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace GuldeEditor.Economy
{
    public class BillingReporter : OdinEditorWindow
    {
        [MenuItem("Gulde/Billing Reporter")]
        static void ShowWindow() => GetWindow<BillingReporter>();

        [OdinSerialize]
        [OnValueChanged("OnWealthChanged")]
        [ValueDropdown("@FindObjectsOfType<WealthComponent>()")]
        WealthComponent Wealth { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Sales { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string TotalRevenue { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Purchases { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Hirings { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Carts { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Repairs { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Wages { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string TotalExpense { get; set; }

        [OdinSerialize]
        [ReadOnly]
        string Total { get; set; }

        void Awake()
        {
            if (!Wealth) return;

            Wealth.Billed -= OnBilled;
            Wealth.Billed += OnBilled;
        }

        void OnWealthChanged()
        {
            if (!Wealth) return;

            Wealth.Billed -= OnBilled;
            Wealth.Billed += OnBilled;
        }

        void OnBilled(object sender, WealthComponent.BilledEventArgs e)
        {
            Debug.Log("Bill updated");

            var total = 0f;
            var totalExpense = 0f;
            var totalRevenue = 0f;

            if (e.Expenses.ContainsKey(WealthComponent.TurnoverType.Purchase))
            {
                var value = e.Expenses[WealthComponent.TurnoverType.Purchase];
                total -= value;
                totalExpense -= value;
                Purchases = $"Wareneink??ufe:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(WealthComponent.TurnoverType.Hiring))
            {
                var value = e.Expenses[WealthComponent.TurnoverType.Hiring];
                total -= value;
                totalExpense -= value;
                Hirings = $"Handgelder:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(WealthComponent.TurnoverType.Cart))
            {
                var value = e.Expenses[WealthComponent.TurnoverType.Cart];
                total -= value;
                totalExpense -= value;
                Carts = $"Fu??gelder:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(WealthComponent.TurnoverType.Repair))
            {
                var value = e.Expenses[WealthComponent.TurnoverType.Repair];
                total -= value;
                totalExpense -= value;
                Repairs = $"Reparaturen & Sanierungen:\t-{value} Gulden";
            }

            if (e.Expenses.ContainsKey(WealthComponent.TurnoverType.Wage))
            {
                var value = e.Expenses[WealthComponent.TurnoverType.Wage];
                total -= value;
                totalExpense -= value;
                Wages = $"Personalkosten:\t-{value} Gulden";
            }

            if (e.Revenues.ContainsKey(WealthComponent.TurnoverType.Sale))
            {
                var value = e.Revenues[WealthComponent.TurnoverType.Sale];
                total += value;
                totalRevenue += value;
                Sales = $"Warenverk??ufe:\t+{value} Gulden";
            }

            Total = $"Gesamt:\t{total} Gulden";
            TotalExpense = $"Ausgaben Gesamt:\t{totalExpense} Gulden";
            TotalRevenue = $"Einnahmen Gesamt:\t{totalRevenue} Gulden";
        }

        bool IsDisabled => !Locator.Time || Locator.Time.IsRunning || !Application.isPlaying;

        [Button]
        [DisableIf("IsDisabled")]
        void Advance()
        {
            Locator.Time.StartTime();
        }
    }
}