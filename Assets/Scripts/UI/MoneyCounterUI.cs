namespace UI
{
	public class MoneyCounterUI : CounterUI
	{
		protected override void UnSubscribe() => PlayerCurrency.OnCurrencyChanged -= Received;

		protected override void Subscribe() => PlayerCurrency.OnCurrencyChanged += Received;
	}
}