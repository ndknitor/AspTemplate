using AutoMapper;
using NewTemplate;
using NewTemplate.Context;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Seat, RSeat>();
    }
}