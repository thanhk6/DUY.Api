using AutoMapper;
using DUY.API.Entities;
using C.Tracking.API.Model;
using C.Tracking.API.Model.Customer;
using C.Tracking.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using DUY.API.IRepositories;
using DUY.API.Model.Customer;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Compression.Zip;
//using Google.Apis.Auth.SignedTokenVerification;

using System.Security.Cryptography.Xml;
using Google.Apis.Auth;
using RestSharp;
using System.Security.Principal;
using System.Text.Json;
using Realms.Sync.Exceptions;
namespace DUY.API.Repositories
{
    public class CustomerRepository : ICustomerRepository

    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IContractFileRepository _file;
        protected readonly IConfiguration _configuration;
        public CustomerRepository(ApplicationContext context, IMapper mapper, IContractFileRepository fileRepository, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _file = fileRepository;
            _configuration = configuration;

        }
        public Task<int> CustomerCheck(string phone)
        {
            int check = _context.Customer.Where(r => r.phone == phone).Count();
            return Task.FromResult(check);
        }
        public async Task<Customer> CustomerGetPhone(string phone)
        {
            Customer customer = new Customer();
            customer = _context.Customer.Where(r => r.phone == phone).FirstOrDefault();
            return customer;
        }
        //public async Task<string> CustomerCreate(CustomerAddModel model)
        //{

        //    return await Task.Run(() =>
        //    {
        //        string respons = "0";
        //        try
        //        {
        //            Customer customer = new Customer
        //            {
        //                pass_code = Encryptor.RandomPassword(),
        //                phone = model.phone,
        //                code = "",
        //                username = model.phone,
        //                referral_code = model.referral_code,
        //                name = model.name
        //            };
        //            customer.password = Encryptor.MD5Hash(model.password + customer.pass_code);
        //            customer.dateAdded = DateTime.Now;
        //            customer.is_active = true;
        //            _context.Customer.Add(customer);
        //            _context.SaveChanges();
        //            customer.code = "SMG_CUS_" + customer.id;
        //            _context.Customer.Update(customer);
        //            _context.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            respons = ex.Message;
        //        }
        //        return Task.FromResult(respons);
        //    });

        //}
        public async Task<Customercreate> CustomerCreate(Customercreate model)
        {
            return await Task.Run(() =>
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Customer Customer = _mapper.Map<Customer>(model);
                        Customer.dateAdded = DateTime.Now;
                        string tablename = Common.TableName.Customer.ToString();

                      

                        _context.Customer.Add(Customer);
                        _context.SaveChanges();
                        transaction.Commit();
                        return model;

                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;

                    }
                }
            });
        }
        public async Task<int> Authenticate(CustomerLoginModel login)
        {
            return 1;
        }

        public async Task<CustomerModel> Customer(long id)
        {
            return await Task.Run(async () =>
            {
                string tablename = Common.TableName.Customer.ToString();
                CustomerModel model = new CustomerModel();
                Customer customer = await _context.Customer.FirstOrDefaultAsync(r => r.id == id && !r.is_delete);
                if (customer != null)
                {
                    model = _mapper.Map<CustomerModel>(customer);

                    model.image = _context.Files.Where(x => x.tablename == tablename && x.idtable == id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault();
                }
                return model;
            });
        }
        public async Task<CustomerModel> CustomerModify(CustomerModel model)
        {
            return await Task.Run(() =>
            {
                //  Customer customer = _mapper.Map<Customer>(model);
                Customer customer = _context.Customer.FirstOrDefault(x => x.id == model.id);
                customer.name = model.name;
                customer.address = model.address;
                customer.phone = model.phone;
                customer.dateUpdated = DateTime.Now;
                string tablename = Common.TableName.Customer.ToString();
                if (model.image != null)
                {
                    var image = _context.Files.AsNoTracking().Where(x => x.tablename == tablename && x.idtable == model.id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault();
                    if (image == null)
                        _file.FileSingleCreate(model.image, tablename, model.id, 1);
                    else
                        _file.FileSingleModify(model.image, tablename, model.id, 1);
                }
                _context.Customer.Update(customer);
                _context.SaveChanges();
                return Task.FromResult(model);
            });
        }
        public async Task<bool> CustomerDelete(long customer_id, long user_id)
        {
            return await Task.Run(() =>
            {
                var customer = _context.Customer.FirstOrDefault(x => x.id == customer_id);
                if (customer != null)
                {
                    customer.is_delete = true;
                    customer.dateUpdated = DateTime.Now;
                    customer.userUpdated = user_id;
                    _context.Customer.Update(customer);
                    _context.SaveChanges();
                }
                return Task.FromResult(true);
            });
        }
        public async Task<PaginationSet<CustomerModel>> CustomerList(CustomerSearch model)
        {
            string tablename = Common.TableName.Customer.ToString();
            await Task.CompletedTask;
            PaginationSet<CustomerModel> response = new PaginationSet<CustomerModel>();
            IQueryable<CustomerModel> listItem = from a in _context.Customer
                                                 where !a.is_delete
                                                 select new CustomerModel
                                                 {
                                                     id = a.id,
                                                     name = a.name,
                                                     phone = a.phone,
                                                     address = a.address,
                                                     code = a.code,
                                                     dateAdded = a.dateAdded,
                                                     userAdded = a.userAdded,
                                                     dateUpdated = a.dateUpdated,
                                                     userUpdated = a.userUpdated,
                                                     image = _context.Files.Where(x => x.tablename == tablename && x.idtable == a.id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault()
                                                 };
            if (model.keyword is not null and not "")
            {
                listItem = listItem.Where(r => r.name.Contains(model.keyword) || r.phone.Contains(model.keyword));
            }
            if (model.page_number > 0)
            {
                response.totalcount = listItem.Select(x => x.id).Count();
                response.page = model.page_number;
                response.maxpage = (int)Math.Ceiling((decimal)response.totalcount / model.page_size);
                response.lists = await listItem.OrderByDescending(r => r.dateAdded).Skip(model.page_size * (model.page_number - 1)).Take(model.page_size).ToListAsync();
            }
            else
            {
                response.lists = await listItem.OrderByDescending(r => r.dateAdded).ToListAsync();
            }
            return response;
        }
        public async Task<CustomerModel> Locginfacebook(FaceebookUserRequest item)
        {
            var model = new CustomerModel();
            // verify access token with facebook API to authenticate
            var client = new RestClient("https://graph.facebook.com/v8.0");

            var request = new RestRequest($"me?access_token={item.IdToken}");

            var response = await client.GetAsync(request);

            //if (!response.IsSuccessful)
            //    throw new AppException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content!);
            var facebookId = long.Parse(data!["id"]);
            var name = data["name"];

            var account = _context.Customer.SingleOrDefault(x => x.facebookId == facebookId);

            // create new account if first time logging in
            if (account != null)
            {             
                model.name = name;
                model.facebookId = account.facebookId;
                return model;
            }
            else
            {
                account = new Customer
                {
                    facebookId = facebookId,
                    name = name,
                    //ExtraInfo = $"This is some extra info about {name} that is saved in the API"
                };
                _context.Customer.Add(account);
                await _context.SaveChangesAsync();
                model.name = name;
                model.facebookId = account.facebookId;
                return model;
            }

            // generate jwt token to access secure routes on this API

        }
        public async Task<CustomerModel> Locgingoogle(GoogleUserRequest item)
        {
            var model = new CustomerModel();
            var setting = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new string[] { "323549604687-d3ilaniehalcph6o3k8p4pajc1bdmtch.apps.googleusercontent.com" }
            };
            var idToken = item.IdToken;
            var result = await GoogleJsonWebSignature.ValidateAsync(idToken, setting);
            if (result != null)
            {
                Customer check = await _context.Customer.FirstOrDefaultAsync(r => r.email == result.Email);
                if (check != null)
                {
                    model.name = check.name;
                    model.phone = check.phone;
                    model.address = check.address;
                    model.code = check.code;
                    model.mail = check.email;
                    return model;
                }
                else
                {
                    Customer _item = new Customer
                    {
                        name = result.Name,
                        code = result.Name,
                        email = result.Email,
                    };
                    _context.Customer.Add(_item);
                    _context.SaveChanges();
                    return model;
                }
            }
            else
            {

                return model;
            }
        }
    }
}
    
