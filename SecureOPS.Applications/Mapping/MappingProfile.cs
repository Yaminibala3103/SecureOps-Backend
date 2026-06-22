using AutoMapper;
using SecureOps.Applications.DTOs;


using SecureOPS.Domain.Enum;
using SecureOPS.Domain.Entities;
using SecureOPS.Applications.DTOs;

namespace SecureOPS.Application.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{


			CreateMap<ThreatIndicator, ThreatIndicatorReadDto>()
			 .ForMember(d => d.ThreatcampaignName, o => o.MapFrom(s => s.Campaign.Name ?? "N/A"));

			// Map CreateDto to Entity while ignoring DB-managed fields
			CreateMap<ThreatIndicatorCreateDto, ThreatIndicator>()
				.ForMember(d => d.IndicatorId, o => o.Ignore())
				.ForMember(d => d.Campaign, o => o.Ignore());

			CreateMap<ThreatCampaign, ThreatCampaignReadDto>();

			CreateMap<ThreatCampaignCreateDto, ThreatCampaign>();
			// .ForMember(x => x.CampaignId, opt => opt.Ignore());


			CreateMap<Vulnerability, VulnerabilityReadDto>();
			CreateMap<VulnerabilityCreateDto, Vulnerability>()
				.ForMember(d => d.CVE, opt => opt.MapFrom(s => s.CVE.ToUpper().Trim()));



			//CreateMap<Remediation, RemediationReadDto>();

			//CreateMap<RemediationCreateDto, Remediation>()
			//    .ForMember(dest => dest.RemediationId, opt => opt.Ignore());
			CreateMap<Remediation, RemediationResponseDto>()
				// Map strings from related entities to prevent ID-only responses
				.ForMember(d => d.CVE, o => o.MapFrom(s => s.Vulnerability != null ? s.Vulnerability.CVE : "N/A"))
				.ForMember(d => d.HostName, o => o.MapFrom(s => s.Asset != null ? s.Asset.HostName : "Unknown"))

				// Convert Enums to readable text (e.g., "InProgress", "Critical")
				.ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
				.ForMember(d => d.Severity, o => o.MapFrom(s => s.Vulnerability != null ? s.Vulnerability.Severity.ToString() : "Low"));

				


			//CreateMap<AssetCreateDto, Asset>();
			//CreateMap<AssetCreateDto, Asset>().ReverseMap();
			CreateMap<Asset, AssetReadDto>();

			// CreateDto -> Entity
			CreateMap<AssetCreateDto, Asset>()
				.ForMember(dest => dest.AssetId, opt => opt.Ignore())
				.ForMember(dest => dest.HostName, opt => opt.MapFrom(src => src.HostName!.ToUpper().Trim()))
				.ForMember(dest => dest.Ipaddress, opt => opt.MapFrom(src => src.Ipaddress!.Trim()))
				.ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner!.ToLower().Trim()));

		//SecurityReport Mappings with Business Logic for Status and Severity


		CreateMap<SecurityReport, ReportDTO>()
		.ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
		src.MetricMttr == 1 ? "Closed" :
		src.MetricMttr == 2 ? "Pending" : "Open"))

	// Replicates your SSMS Severity logic: MTTD > 10 is High
	.ForMember(dest => dest.Severity, opt => opt.MapFrom(src =>
		src.MetricMttd > 10 ? "High" : "Low"));


		CreateMap<ReportDTO, SecurityReport>().ForMember(dest => dest.GeneratedBy, opt => opt.MapFrom(src => src.GeneratedBy))
	.ForMember(dest => dest.GeneratedDate, opt => opt.MapFrom(src => DateTime.Now));

		CreateMap<CreateReportDTO, SecurityReport>().ReverseMap();
		CreateMap<UpdateReportDTO, SecurityReport>().ReverseMap();
		CreateMap<SecurityReport, ReportResponseDTO>()
	.ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
		src.MetricMttr == 1 ? "Closed" :
		src.MetricMttr == 2 ? "Pending" : "Open"))
	.ForMember(dest => dest.Severity, opt => opt.MapFrom(src =>
		src.MetricMttd > 10 ? "High" : "Low"));
		CreateMap<ReportResponseDTO, SecurityReport>();
		CreateMap<SecurityReport, ReportDTO>().ReverseMap();


		CreateMap<AuditLog, AuditLogDto>().ReverseMap();







		}
	}
}