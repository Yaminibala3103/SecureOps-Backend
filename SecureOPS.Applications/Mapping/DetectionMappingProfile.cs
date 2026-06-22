using AutoMapper;
using SecureOPS.Applications.DTOs;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.Mapping
{
    public class DetectionMappingProfile : Profile
    {
        public DetectionMappingProfile()
        {
            CreateMap<SecurityEvent, SecurityEventDto>().ReverseMap();
            CreateMap<Alert, AlertDto>().ReverseMap();
            CreateMap<Notification, NotificationDto>().ReverseMap();

            CreateMap<Incident, IncidentDto>().ReverseMap();
            CreateMap<IncidentTask, IncidentTaskDto>().ReverseMap();
        }
    }
}
