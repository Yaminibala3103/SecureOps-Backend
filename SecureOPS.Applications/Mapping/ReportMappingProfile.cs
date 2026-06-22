using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using SecureOPS.Applications.DTOs;


using SecureOPS.Domain.Enum;
using SecureOPS.Domain.Entities;


public class ReportMappingProfile : Profile
{
	public ReportMappingProfile()
	{


		CreateMap<SecurityReport, ReportResponseDTO>();
			

	
		CreateMap<CreateReportDTO, SecurityReport>()
			.ForMember(dest => dest.GeneratedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
			.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

	
		CreateMap<UpdateReportDTO, SecurityReport>()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		// 4. Basic Internal Mappings
		CreateMap<SecurityReport, ReportDTO>().ReverseMap();
		CreateMap<AuditLog, AuditLogDto>().ReverseMap();
		CreateMap<ReportResponseDTO, SecurityReport>();
	}
}

