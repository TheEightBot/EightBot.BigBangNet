using System;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;

namespace EightBot.BigBang.XamForms
{
    public static class PageExtensions
    {
        public static bool IsPresentedModally (this Page page) {
            return page != null && page.Navigation != null && page.Navigation.ModalStack.Contains (page);
        }

        public static bool IsPresentedOnStack (this Page page)
        {
            return page != null && page.Navigation != null && page.Navigation.NavigationStack.Contains (page);
        }

        public static void GenerateViewNames(this IEnableLogger logger, Page page)
        {
            var missingViewsCount = 0;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var pageType = page.GetType();

            var diagnosticsOutput = new StringBuilder();

            diagnosticsOutput.AppendLine(string.Format("----- Missing View IDs for {0}  - Start -----", pageType.Name));

            var properties = pageType.GetRuntimeProperties().ToList();

            if (properties.Any())
            {
                diagnosticsOutput.AppendLine("\t[ Generated IDs for Properties ]");
                
                foreach (var property in properties)
                {
                    try
                    {
                        if (property.GetValue(page) is VisualElement ve)
                        {
                            if (string.IsNullOrEmpty(ve.AutomationId))
                            {
                                ve.AutomationId = property.Name;
                                diagnosticsOutput.AppendLine(string.Format("\t{0}.AccessibilityIdentifier = \"{0}\";", property.Name));
                                missingViewsCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        diagnosticsOutput.AppendLine(string.Format("\tUnable to Bind: {0}", ex));
                    }
                }

                diagnosticsOutput.AppendLine();
            }

            var fields = pageType.GetRuntimeFields();

            if (fields.Any())
            {
                diagnosticsOutput.AppendLine("\t[ Generated IDs for Fields ]");

                foreach (var field in fields)
                {
                    try
                    {
                        if (field.GetValue(page) is VisualElement ve)
                        {
                            if (string.IsNullOrEmpty(ve.AutomationId))
                            {
                                ve.AutomationId = field.Name;
                                diagnosticsOutput.AppendLine(string.Format("\t{0}.AutomationId = \"{0}\";", field.Name));
                                missingViewsCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        diagnosticsOutput.AppendLine(string.Format("\tUnable to Bind: {0}", ex));
                    }
                }

                diagnosticsOutput.AppendLine();
            }
            stopwatch.Stop();

            if (missingViewsCount == 0)
                return;

            diagnosticsOutput.AppendLine(string.Format("\t[ Processing Time for {0} : {1}ms ]", pageType.Name, stopwatch.ElapsedMilliseconds));
            diagnosticsOutput.AppendLine(string.Format("----- Found {0} Missing View IDs for {1}  - End -----", missingViewsCount, pageType.Name));
            diagnosticsOutput.AppendLine();
            
            logger.Log().Debug(diagnosticsOutput.ToString());
        }

        public static async Task PopTo<TPage>(this VisualElement visualElement, bool animated = true) where TPage : Page
        {
            var navStack = visualElement?.Navigation?.NavigationStack;
            if (!navStack?.Any() ?? true)
                return;

            for (int i = navStack.Count - 1; i >= 0; i--) {
                var currPage = navStack[i];

                if (currPage == visualElement)
                    continue;

                if (currPage is TPage) {
                    await visualElement.Navigation.PopAsync(animated);
                    return;
                }

                visualElement.Navigation.RemovePage(currPage);
            }
        }
    
        public static async Task AppearingAsync(this Page page)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                page.Appearing += Page_Appearing;

                await tcs.Task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            finally 
            { 
                page.Appearing -= Page_Appearing;
            }

            void Page_Appearing(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }
        }

        public static async Task DisappearingAsync(this Page page)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                page.Disappearing += Page_Disappearing;

                await tcs.Task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
            finally
            {
                page.Disappearing -= Page_Disappearing;
            }

            void Page_Disappearing(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }
        }


    }
}

