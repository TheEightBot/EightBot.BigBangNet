using System;
using System.Threading.Tasks;
using EightBot.BigBang.Interfaces;
using Splat;
using Microsoft.Maui;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class BackgroundProcessorPage : ContentPage
    {
        public BackgroundProcessorPage()
        {
            var stackLayout = new StackLayout { };

            var output = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var button = new Button
            {
                Text = "Process In Background",
                Command = new Command(async () =>
                {
                    var backgroundProcessor = Locator.Current.GetService<IBackgroundTask>();

                    output.Text = "Starting Processing";

                    await backgroundProcessor.ProcessInBackground(async () =>
                    {
                        await Task.Delay(500);
                    });

                    output.Text = "Finished No Output Processing";

                    await Task.Delay(250);

                    var result = await backgroundProcessor.ProcessInBackground(async () =>
                    {
                        await Task.Delay(2500);

                        System.Diagnostics.Debug.WriteLine("Finished Background Processing");
                        return Guid.NewGuid().ToString();
                    });

                    output.Text = $"Finished No Output Processing With Result: {result}";
                })
            };

            stackLayout.Children.Add(button);
            stackLayout.Children.Add(output);


            this.Content = stackLayout;

        }
    }
}
