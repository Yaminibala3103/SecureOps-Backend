using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Domain
{
	public class ErrorResponse
	{
		public int StatusCode { get; set; }
		public string Message { get; set; } = string.Empty;
		public string? Details { get; set; } // Only shown in Development mode


		public ErrorResponse() { }
		public ErrorResponse(int statusCode, string message, string? details = null)
		{
			StatusCode = statusCode; Message = message; Details = details;
		}
	}
}
