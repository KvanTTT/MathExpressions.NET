using System;

namespace MathExpressionsNET
{
	public enum ErrorType
	{
		Lexer,
		Parser,
		Converter
	}

	public class ErrorMessage : IEquatable<ErrorMessage>, IComparable<ErrorMessage>
	{
		public int Line { get; set; }
		public int Column { get; set; }
		public int Offset { get; set; }
		public string Message { get; set; }
		public string SourceLine { get; set; }
		public ErrorType ErrorType { get; set; }
		public string FileName { get; set; }

		public bool Equals(ErrorMessage other)
		{
			if (other == null)
				return false;

			return Line == other.Line &&
				   Column == other.Column &&
				   Offset == other.Offset &&
				   Message == other.Message &&
				   ErrorType == other.ErrorType &&
				   FileName == other.FileName;
		}

		public int CompareTo(ErrorMessage other)
		{
			int fileNameComparison = string.CompareOrdinal(FileName, other.FileName);

			if (fileNameComparison == 0)
			{
				int errorTypeComparison = ErrorType.CompareTo(other.ErrorType);
				if (errorTypeComparison == 0)
				{
					return Offset != 0 && other.Offset != 0
						? Offset.CompareTo(other.Offset)
						: ((Line * 1000 + other.Column).CompareTo(Line * 1000 + other.Column));
				}
				return errorTypeComparison;
			}
			return fileNameComparison;
		}

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(FileName) ? Message : $"{FileName}: {Message}";
		}
	}
}
