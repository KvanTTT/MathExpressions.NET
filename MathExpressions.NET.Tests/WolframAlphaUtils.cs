using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolframAlphaNET;
using WolframAlphaNET.Misc;
using WolframAlphaNET.Objects;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace MathExpressionsNET.Tests
{
	public static class WolframAlphaUtils
	{
		private static string WolframAlphaAppId;
		
		static WolframAlphaUtils()
		{
			InitWolframAlphaAppId();
		}

		static void InitWolframAlphaAppId([CallerFilePath] string path = "")
		{
			WolframAlphaAppId = File.ReadAllText(Path.Combine(Path.GetDirectoryName(path), "WolframAlphaAppId")).Trim();
		}
		
		public static bool CheckDerivative(string expression, string derivative)
		{
			return CheckEquality("diff(" + expression + ")", derivative);
		}

		public static bool CheckEquality(string expression1, string expression2)
		{
			WolframAlpha wolfram = new WolframAlpha(WolframAlphaAppId);

			string query = "(" + expression1.Replace(" ", "") + ")-(" + expression2.Replace(" ", "") + ")";
			QueryResult result = wolfram.Query(query);
			result.RecalculateResults();

			try
			{
				return double.TryParse(result.GetPrimaryPod().SubPods[0].Plaintext, out double d) && d == 0.0;
			}
			catch
			{
				return false;
			}
		}

		public static bool CheckDerivative2(string expression, string derivative)
		{
			try
			{
				WolframAlpha wolfram = new WolframAlpha(WolframAlphaAppId);

				QueryResult result = wolfram.Query("diff(" + expression + ")");
				string answer = result.Pods[0].SubPods[0].Plaintext;
				string answerDer = answer.Remove(0, answer.IndexOf(" = ") + 3).Trim().Replace(" ", "");

				result = wolfram.Query(derivative);
				var pod = result.GetPrimaryPod();

				if (answerDer == pod.SubPods[0].Plaintext.Replace(" ", ""))
					return true;

				pod = result.Pods.Where(p => p.ID == "AlternateForm").FirstOrDefault();

				return answerDer == pod.SubPods[0].Plaintext.Replace(" ", "");
			}
			catch
			{
				return false;
			}
		}

		public static double GetValue(string expression, params KeyValuePair<string, double>[] values)
		{
			StringBuilder request = new StringBuilder(expression + " where ");
			foreach (var value in values)
				request.AppendFormat("{0}={1};", value.Key, value.Value.ToString(CultureInfo.InvariantCulture));
			if (values.Length > 0)
				request.Remove(request.Length - 1, 1);

			WolframAlpha wolfram = new WolframAlpha(WolframAlphaAppId);
			QueryResult response = wolfram.Query(request.ToString());
			var pod = response.GetPrimaryPod();

			return double.Parse(pod.SubPods[0].Plaintext, CultureInfo.InvariantCulture);
		}
	}
}
