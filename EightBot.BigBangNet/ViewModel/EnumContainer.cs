using System;
using ReactiveUI;

namespace EightBot.BigBang.ViewModel
{
	public class KeyValueContainer<TKey, TValue> : ViewModelBase
	{
		public KeyValueContainer ()
		{
		}

		TKey _key;
		public TKey Key {
			get { return _key; }
			set { this.RaiseAndSetIfChanged(ref _key, value); }
		}

		TValue _value;
		public TValue Value {
			get { return _value; }
			set { this.RaiseAndSetIfChanged(ref _value, value); }
		}

		ObservableAsPropertyHelper<string> _title;
		public override string Title {
			get {
				return _title?.Value ?? string.Empty;
			}
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			Key = default(TKey);
			Value = default(TValue);
		}

		protected override void RegisterObservables ()
		{
			this.WhenAny (x => x.Key, x => x.ToString())
				.ToProperty (this, x => x.Title, out _title);
		}
	}
}

