using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.Calculators
{
	public  class ReportMetricResult
	{


		public int IncidentCount { get; set; }
		public double Mttd { get; set; }
		public double Mttr { get; set; }
		public double RiskScore { get; set; }
		
	}
}
