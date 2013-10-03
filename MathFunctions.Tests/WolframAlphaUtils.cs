using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using WolframAlphaNET;
using WolframAlphaNET.Misc;
using WolframAlphaNET.Objects;

namespace MathFunctions.Tests
{
	public static class WolframAlphaUtils
	{
		public static bool CheckDerivative(string expression, string derivative)
		{
			return CheckEquality("diff(" + expression + ")", derivative);
			//return CheckDerivative2(expression, derivative);
		}

		public static bool CheckEquality(string expression1, string expression2)
		{
			WolframAlpha wolfram = new WolframAlpha(ConfigurationManager.AppSettings["WolframAlphaAppId"]);

			string query = expression1.Replace(" ", "") + "===" + expression2.Replace(" ", "");
			QueryResult result = wolfram.Query(query);
			result.RecalculateResults();

			try
			{
				return Convert.ToBoolean(result.GetPrimaryPod().SubPods[0].Plaintext);
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
				WolframAlpha wolfram = new WolframAlpha(ConfigurationManager.AppSettings["WolframAlphaAppId"]);

				QueryResult result = wolfram.Query("diff(" + expression + ")");
				string answer = result.Pods[0].SubPods[0].Plaintext;
				string answerDer = answer.Remove(0, answer.IndexOf(" = ") + 3).Trim();

				result = wolfram.Query(derivative);
				var pod = result.GetPrimaryPod();
				if (pod == null)
					result.Pods.Where(p => p.ID == "AlternateForm").FirstOrDefault();
				var formattedDer = pod.SubPods[0].Plaintext;

				return answerDer == formattedDer;
			}
			catch
			{
				return false;
			}
		}
	}
}
