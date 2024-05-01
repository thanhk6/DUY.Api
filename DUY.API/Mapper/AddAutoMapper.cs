using AutoMapper;
using DUY.API.Entities;
using DUY.API.Model.Song;
using DUY.API.Model.Customer;
using DUY.API.Model.Contract;

namespace C.Tracking.API.Mapper
{
    public class AddAutoMapper : Profile
    {
        public AddAutoMapper()
        {
            CreateMap<CustomerModel, Customer>();
            CreateMap<Customer, CustomerModel>();

            CreateMap<Customercreate, Customer>();
            CreateMap<Customer, Customercreate>();


            CreateMap<ComMentModel, ComMent>();

            CreateMap<ComMent, ComMentModel>();

            CreateMap<Songmodel, Song>();

            CreateMap<Song, Songmodel>();






        }
    }
}
