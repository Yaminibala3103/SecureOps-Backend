using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException()
			: base("The requested resource was not found.") { }

		public NotFoundException(string message)
			: base(message) { }
	}

	// Renamed to avoid conflict with System.ComponentModel.DataAnnotations.ValidationException
	public class AppValidationException : Exception
	{
		public AppValidationException()
			: base("Validation failed.") { }

		public AppValidationException(string message)
			: base(message) { }
	}

	public class ForbiddenException : Exception
	{
		public ForbiddenException()
			: base("Access is forbidden.") { }

		public ForbiddenException(string message)
			: base(message) { }
	}
}
