using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DataModder
{
	public class TextBoxAppendTraceListener : TraceListener
	{
		private TextBox _textBox;

		public TextBoxAppendTraceListener(TextBox textBox)
		{
			_textBox = textBox;
		}

		public override void Write(string message)
		{
			InvokeIfRequired(_textBox, (MethodInvoker)delegate
			{
				_textBox.AppendText(message);
			});
		}

		public static void InvokeIfRequired(Control control, MethodInvoker action)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}

		public override void WriteLine(string message)
		{
			this.Write(message + Environment.NewLine);
		}
	}
}
