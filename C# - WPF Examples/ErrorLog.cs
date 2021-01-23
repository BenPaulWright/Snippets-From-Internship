using System;
using System.Windows;

namespace SpectroVision_Setup_Wizard.OtherClasses
{
    /// <summary>
    ///     Error Message Format:
    ///     ---------------------------
    ///     | Error in {ErrorThrower} |
    ///     | {ErrorMessage}          |
    ///     | {SystemError}           |
    ///     ---------------------------
    /// </summary>
    public class ErrorLog
    {
        [Flags]
        public enum Output
        {
            Console = 1 << 0,
            MessageBox = 1 << 1,
            FileLog = 1 << 2,
            Event = 1 << 3
        }

        private readonly Output _internalOutput;

        public ErrorLog(Output output)
        {
            _internalOutput = output;
        }

        public void LogError(string errorThrower = null, string errorMessage = null, string systemError = null)
        {
            if (_internalOutput.HasFlag(Output.Console))
            {
                if (errorThrower != null)
                    Console.WriteLine(errorThrower);
                if (errorMessage != null)
                    Console.WriteLine(errorMessage);
                if (systemError != null)
                    Console.WriteLine(systemError);
            }

            if (_internalOutput.HasFlag(Output.MessageBox))
            {
                var mbMessage = "";

                if (errorMessage != null)
                    mbMessage += errorMessage + "\n\n";

                if (systemError != null)
                    mbMessage += systemError;


                if (errorThrower != null)
                    MessageBox.Show(mbMessage, errorThrower);
                else
                    MessageBox.Show(mbMessage);
            }

            if (_internalOutput.HasFlag(Output.FileLog))
            {
                //throw new NotImplementedException();
            }

            if (_internalOutput.HasFlag(Output.Event))
            {
                ErrorLogged?.Invoke(errorThrower, errorMessage, systemError);
            }
        }

        public delegate void ErrorLoggedDelegate(string errorThrower = null, string errorMessage = null, string systemError = null);
        public event ErrorLoggedDelegate ErrorLogged;

        public void LogException(string errorThrower = null, Exception exception = null)
        {
            if (_internalOutput.HasFlag(Output.Console))
            {
                if (errorThrower != null)
                    Console.WriteLine(errorThrower);
                if (exception != null)
                    Console.WriteLine($@"Exception Thrown: {exception}");
            }

            if (_internalOutput.HasFlag(Output.MessageBox))
            {
                var mbMessage = "";

                if (exception != null)
                    mbMessage += $@"Exception Thrown: {exception}";


                if (errorThrower != null)
                    MessageBox.Show(mbMessage, errorThrower);
                else
                    MessageBox.Show(mbMessage);
            }

            if (_internalOutput.HasFlag(Output.FileLog))
            {
                //throw new NotImplementedException();
            }

            if (_internalOutput.HasFlag(Output.Event))
            {
                ExceptionLogged?.Invoke(errorThrower, exception);
            }
        }

        public delegate void ExceptionLoggedDelegate(string errorThrower = null, Exception exception = null);
        public event ExceptionLoggedDelegate ExceptionLogged;

        public void LogMessage(string messageThrower = null, string message = null)
        {
            if (_internalOutput.HasFlag(Output.Console))
            {
                if (messageThrower != null)
                    Console.WriteLine(messageThrower);
                if (message != null)
                    Console.WriteLine(message);
            }

            if (_internalOutput.HasFlag(Output.MessageBox))
            {
                if (message == null)
                    message = "";

                if (messageThrower != null)
                    MessageBox.Show(message, messageThrower);
                else
                    MessageBox.Show(message);
            }

            if (_internalOutput.HasFlag(Output.FileLog))
            {
                //throw new NotImplementedException();
            }

            if (_internalOutput.HasFlag(Output.Event))
            {
                MessageLogged?.Invoke(messageThrower, message);
            }
        }

        public delegate void MessageLoggedDelegate(string messageThrower = null, string message = null);
        public event MessageLoggedDelegate MessageLogged;
    }
}