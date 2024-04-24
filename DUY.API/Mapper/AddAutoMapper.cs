using AutoMapper;
using DUY.API.Entities;
using C.Tracking.API.Model.User;

using DUY.API.Model.Song;
using DUY.API.Model.User;
using DUY.API.Model.Customer;
using DUY.API.Model.Contract;

namespace C.Tracking.API.Mapper
{
    public class AddAutoMapper : Profile
    {
        public AddAutoMapper()
        {
            CreateMap<Admin_User, UserCreateModel>();
            CreateMap<UserCreateModel, Admin_User>();

            CreateMap<Admin_User, UserModifyModel>();
            CreateMap<UserModifyModel, Admin_User>();

            CreateMap<Admin_User, UserModel>();



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
