using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EightBot.BigBang.Services
{
    public class DebugDiagnosticsAnalytics : Interfaces.IAnalytics
    {
        bool _isDebug = false;

        #region IAnalyticsService implementation

        public bool Enabled
        {
            get;
            set;
        }

        public void Init()
        {
#if DEBUG
            Enabled = _isDebug = true;
#endif
        }

        public void SetUser(string identifier, Dictionary<string, string> userProperties)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine("[ Set User - Start ]");
                diagnosticOutput.AppendLine(String.Format("\tIdentifier: {0}", identifier));
                if (userProperties != null && userProperties.Any())
                {
                    foreach (var userProperty in userProperties)
                        diagnosticOutput.AppendLine(String.Format("\t: {0} - {1}", userProperty.Key, userProperty.Value));
                }
                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine("[ Set User - End ]");
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogViewLoaded(string name)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - Start ]", EightBot.BigBang.Constants.Tracking.ViewLoaded));

                diagnosticOutput.AppendLine(String.Format("\tName: {0}", name));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - End ]", EightBot.BigBang.Constants.Tracking.ViewLoaded));
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogViewUnloaded(string name)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - Start ]", EightBot.BigBang.Constants.Tracking.ViewUnloaded));

                diagnosticOutput.AppendLine(String.Format("\tName: {0}", name));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - End ]", EightBot.BigBang.Constants.Tracking.ViewUnloaded));
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogViewDisplayed(string name)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - Start ]", EightBot.BigBang.Constants.Tracking.ViewDisplayed));

                diagnosticOutput.AppendLine(String.Format("\tName: {0}", name));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - End ]", EightBot.BigBang.Constants.Tracking.ViewDisplayed));
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogViewHidden(string name)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - Start ]", EightBot.BigBang.Constants.Tracking.ViewHidden));

                diagnosticOutput.AppendLine(String.Format("\tName: {0}", name));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - End ]", EightBot.BigBang.Constants.Tracking.ViewHidden));
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogError(string name, string title, string message)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - Start ]", EightBot.BigBang.Constants.Tracking.UserError));

                diagnosticOutput.AppendLine(
                    String.Format("\t{0}: {1}", EightBot.BigBang.Constants.Tracking.UserErrorView, name));

                diagnosticOutput.AppendLine(
                    String.Format("\t{0}: {1}", EightBot.BigBang.Constants.Tracking.UserErrorTitle, title));

                diagnosticOutput.AppendLine(
                    String.Format("\t{0}: {1}", EightBot.BigBang.Constants.Tracking.UserErrorMessage, message));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine(
                    String.Format("[ {0} - End ]", EightBot.BigBang.Constants.Tracking.UserError));
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }

        }

        public void LogEvent(string name, string category, string eventName, string eventData)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine("[ Tracking Event - Start ]");

                diagnosticOutput.AppendLine(String.Format("\tName: {0}", name));
                diagnosticOutput.AppendLine(String.Format("\tCategory: {0}", category));
                diagnosticOutput.AppendLine(String.Format("\tEvent Name: {0}", eventName));
                diagnosticOutput.AppendLine(String.Format("\tEvent Data: {0}", eventData));

                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine("[ Tracking Event - End ]");
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }

        public void LogPerformance(string category, TimeSpan performanceCounter, Dictionary<string, string> values = null)
        {
            if (Enabled && _isDebug)
            {
                var diagnosticOutput = new StringBuilder();
                diagnosticOutput.AppendLine("[ Performance Tracking - Start ]");
                diagnosticOutput.AppendLine(String.Format("\tIdentifier: {0}", category));
                diagnosticOutput.AppendLine(String.Format("\tTotal Duration: {0}ms", performanceCounter.TotalMilliseconds));

                if (values != null && values.Any())
                {
                    foreach (var userProperty in values)
                        diagnosticOutput.AppendLine(String.Format("\t: {0} - {1}", userProperty.Key, userProperty.Value));
                }
                diagnosticOutput.AppendLine(String.Format("\tLog Time: {0}", DateTime.Now));
                diagnosticOutput.AppendLine("[ Performance Tracking - End ]");
                diagnosticOutput.AppendLine();

                System.Diagnostics.Debug.WriteLine(diagnosticOutput.ToString());
            }
        }
        #endregion
    }
}

