using DUY.API.Entities;
using C.Tracking.API.Model;
using C.Tracking.API.Model.Customer;
using C.Tracking.API.Model.User;
using DUY.API.Model.Customer;

namespace DUY.API.IRepositories
{
    public interface ICustomerRepository
    {
        Task<int> CustomerCheck(string phone);
        Task<Customer> CustomerGetPhone(string phone);
        Task<int> Authenticate(CustomerLoginModel login);
        Task<Customercreate> CustomerCreate(Customercreate model);

        Task<CustomerModel> Customer(long id);     
        Task<PaginationSet<CustomerModel>> CustomerList(CustomerSearch model);
        Task<CustomerModel> CustomerModify(CustomerModel model);
        Task<bool> CustomerDelete(long customer_id, long user_id);
        Task<CustomerModel> Locginfacebook(FaceebookUserRequest item);
        Task<CustomerModel> Locgingoogle(GoogleUserRequest item);
        
       
    }
}
