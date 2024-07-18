using System;
using Xamarin.Forms;
using System.Text;
using System.Reflection;
using System.Linq;
using Splat;
namespace EightBot.BigBang.XamForms
{
    public static class ContentViewExtensions
    {
        public static void GenerateViewNames(this IEnableLogger logger, ContentView view)
        {
            var missingViewsCount = 0;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var pageType = view.GetType();

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
                        if (property.GetValue(view) is VisualElement ve)
                        {
                            if (string.IsNullOrEmpty(ve.AutomationId))
                            {
                                AutomationProperties.SetIsInAccessibleTree(ve, true);
                                AutomationProperties.SetName(ve, property.Name);

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
                        if (field.GetValue(view) is VisualElement ve)
                        {
                            if (string.IsNullOrEmpty(ve.AutomationId))
                            {
                                AutomationProperties.SetIsInAccessibleTree(ve, true);
                                AutomationProperties.SetName(ve, field.Name);
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
    }
}
