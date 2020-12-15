using AutoMapper;
using Bottom_API.DTO;

namespace Bottom_API.Helpers.AutoMapper
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile() {
            CreateMap<HistoryReportInputDB, HistoryInputReport>();
            CreateMap<HistoryReportOutputDB, HistoryOutputReport>();
        }
    }
}