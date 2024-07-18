namespace EightBot.BigBangNet.MauiSample;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();

        DataList.ItemsSource =
            new List<string>()
            {
                "Test 1",
                "Test 2",
                "Test 3",
                "Test 4",
                "Test 5",
            };
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        Console.WriteLine($"Page Property: {propertyName}");
    }
}