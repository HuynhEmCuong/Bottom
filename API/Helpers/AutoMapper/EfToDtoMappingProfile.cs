using AutoMapper;
using Bottom_API.DTO;
using Bottom_API.Models;

namespace Bottom_API.Helpers.AutoMapper
{
    public class EfToDtoMappingProfile : Profile
    {
        public EfToDtoMappingProfile()
        {
            CreateMap<WMSB_QRCode_Main, QRCode_Main_Dto>();
            CreateMap<WMSB_Packing_List, Packing_List_Dto>();
            CreateMap<WMSB_RackLocation_Main, RackLocation_Main_Dto>();
            CreateMap<WMSB_PackingList_Detail, Packing_List_Detail_Dto>();
            CreateMap<WMSB_QRCode_Detail, QRCode_Detail_Dto>();
            CreateMap<WMSB_Material_Purchase, Material_Dto>()
                .ForMember(dest => dest.Updated_By, opt => opt.MapFrom(src => src.Update_By))
                .ForMember(dest => dest.Updated_Time, opt => opt.MapFrom(src => src.Update_Time)) ;
            CreateMap<WMSB_Material_Missing, Material_Dto>();
            CreateMap<WMSB_Material_Purchase, Receiving_Dto>();
            CreateMap<WMSB_Material_Missing, Receiving_Dto>();
            CreateMap<WMSB_Transaction_Detail, TransferLocationDetail_Dto>();
            CreateMap<WMSB_Material_Sheet_Size, Material_Sheet_Size_Dto>();
            CreateMap<WMSB_Transaction_Main, Transaction_Main_Dto>();
            CreateMap<WMSB_Transfer_Form, Transfer_Form_Dto>();
            CreateMap<WMSB_Setting_Supplier, Setting_Mail_Supplier_Dto>();
        }
    }
}