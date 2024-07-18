using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using EightBot.BigBang;
using EightBot.BigBang.Interfaces;
using EightBot.BigBang.Maui;
using EightBot.BigBang.Sample.Models;
using EightBot.BigBang.SampleApp.UserInterface.Pages;
using ReactiveUI;
using Splat;
using IDeviceInfo = Microsoft.Maui.Devices.IDeviceInfo;

namespace EightBot.BigBang.MauiSample;

public class App : ApplicationBase
{
    public App() : base()
    {
        var debugLogger = new LoggingService { };
        Locator.CurrentMutable.RegisterConstant<ILogger>(debugLogger);

        RxApp.DefaultExceptionHandler = new ExceptionHandler();

        EightBot.BigBang.GlobalConfiguration.LogPerformanceMetrics = true;
        EightBot.BigBang.GlobalConfiguration.GenerateUITestViewNames = true;

        MainPage = new NavigationPage(new SampleAppPage());
    }

    protected override void OnStart()
    {

        // Locator.CurrentMutable.Register(
        //     () => new DiskDataCache(Locator.Current.GetService<IDeviceInfo>()),
        //     typeof(IDataCache));
        //
        // var cache = Locator.Current.GetService<IDataCache>();

        var sm = new SampleModel { StringProperty = "Testing" };

        //Observable
        //.Range(0, 10)
        //.SubscribeAsync(async x =>
        //{
        //    await Task.Delay(10);
        //    System.Diagnostics.Debug.WriteLine($"Processing: {x}");
        //});

        var concurrent = 0;

        //Observable
        //    .Range(0, 200)
        //    .SubscribeAsync(async x =>
        //    {
        //        Interlocked.Increment(ref concurrent);
        //        System.Diagnostics.Debug.WriteLine($"item: {x} - concurrent: {concurrent}");

        //        await Task.Delay(new Random(Guid.NewGuid().GetHashCode()).Next(10, 250));

        //        Interlocked.Decrement(ref concurrent);
        //}, 6);

        Observable
            .Range(0, 200)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SelectConcurrent(
                x =>
                {
                    Interlocked.Increment(ref concurrent);
                    System.Diagnostics.Debug.WriteLine($"item: {x} - concurrent: {concurrent}");

                    Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(10, 250));

                    Interlocked.Decrement(ref concurrent);
                }, 4)
            .Subscribe();

        //Observable
        //.Range(0, 10)
        //.SubscribeAsync(async x =>
        //{
        //    await Task.Delay(new Random(Guid.NewGuid().GetHashCode()).Next(10, 250));
        //    System.Diagnostics.Debug.WriteLine($"Concurrent - 1: {x}");
        //}, 1);

        // cache
        //     .StoreItemToCache(sm, "test")
        //     .ContinueWith(async x => {
        //         await x;
        //
        //         var found = await cache.GetItemFromCache<SampleModel>("test");
        //
        //         System.Diagnostics.Debug.WriteLine($"Found: {found != null}");
        //
        //         var removeResult = await cache.RemoveItemFromCache<SampleModel>("test");
        //
        //         System.Diagnostics.Debug.WriteLine($"Deleted: {removeResult}");
        //
        //         await cache.ClearCache();
        //     });

        // Handle when your app starts
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
    }

    protected override void OnResume()
    {
        // Handle when your app resumes
    }

    protected override void SetupServices()
    {
    }
}

public class ExceptionHandler : IObserver<Exception>
{
    public void OnNext(Exception value)
    {
        if (Debugger.IsAttached)
            Debugger.Break();

        RxApp.MainThreadScheduler.Schedule(() => { throw value; });
    }

    public void OnError(Exception error)
    {
        if (Debugger.IsAttached)
            Debugger.Break();
    }

    public void OnCompleted()
    {
        if (Debugger.IsAttached)
            Debugger.Break();
    }
}

public class LoggingService : ILogger
{
    public Splat.LogLevel Level { get; set; }

    public void Write(string message, Splat.LogLevel logLevel)
    {
        if ((int)logLevel < (int)Level)
        {
            return;
        }

        switch (logLevel)
        {
            case Splat.LogLevel.Warn:
            case Splat.LogLevel.Error:
            case Splat.LogLevel.Fatal:
            case Splat.LogLevel.Debug:
            case Splat.LogLevel.Info:
            default:
                var fio = message.IndexOf(':') + 2;
                Debug.WriteLine(message.Substring(fio));
                break;
        }
    }

    public void Write(Exception exception, string message, LogLevel logLevel)
    {
        if ((int)logLevel < (int)Level)
        {
            return;
        }

        switch (logLevel)
        {
            case Splat.LogLevel.Warn:
            case Splat.LogLevel.Error:
            case Splat.LogLevel.Fatal:
            case Splat.LogLevel.Debug:
            case Splat.LogLevel.Info:
            default:
                var fio = message.IndexOf(':') + 2;
                Debug.WriteLine(message.Substring(fio));
                break;
        }
    }

    public void Write(string message, Type type, LogLevel logLevel)
    {
        if ((int)logLevel < (int)Level)
        {
            return;
        }

        switch (logLevel)
        {
            case Splat.LogLevel.Warn:
            case Splat.LogLevel.Error:
            case Splat.LogLevel.Fatal:
            case Splat.LogLevel.Debug:
            case Splat.LogLevel.Info:
            default:
                var fio = message.IndexOf(':') + 2;
                Debug.WriteLine(message.Substring(fio));
                break;
        }
    }

    public void Write(Exception exception, string message, Type type, LogLevel logLevel)
    {
        if ((int)logLevel < (int)Level)
        {
            return;
        }

        switch (logLevel)
        {
            case Splat.LogLevel.Warn:
            case Splat.LogLevel.Error:
            case Splat.LogLevel.Fatal:
            case Splat.LogLevel.Debug:
            case Splat.LogLevel.Info:
            default:
                var fio = message.IndexOf(':') + 2;
                Debug.WriteLine(message.Substring(fio));
                break;
        }
    }
}