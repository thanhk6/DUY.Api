using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using C.Tracking.API.Extensions;
using C.Tracking.API.Model;
using C.Tracking.API.Model.Customer;
using C.Tracking.Framework.Validator;
using DUY.API.Entities;
using DUY.API.IRepositories;
using DUY.API.Model.Customer;
using DUY.API.Repositories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DUY.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomerRepository _customerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<string> customer_role = new List<string>{"CATEGORYSTOREDETAIL", "BUSINESSINFODETAIL", "CONTRACTCREATE", "CONTRACTMODIFY", "CONTRACTCHANGESTATUS", "CONTRACTDETAIL", "CONTRACTLIST",
                                            "CONTRACTGENPDF", "CUSTOMERDETAIL", "CUSTOMERMODIFY", "BUSINESSINFO", "ADMINSTOCKINFO", "ADMINDASHBOARDINFO", "TRANSFERDETAIL", "TRANSFERLIST",
                                            "TRANSFERMODIFY", "TRANSFERCREATE"};
        public CustomerController(ICustomerRepository customerRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        private string GenerateToken(CustomerClaimModel user)
        {
            var identity = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenSettings:Key"]));
            var token = new JwtSecurityToken(
            _configuration["TokenSettings:Issuer"],
             _configuration["TokenSettings:Audience"],
              expires: DateTime.Now.AddDays(3),
              claims: identity,
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
              );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private IEnumerable<Claim> GetClaims(CustomerClaimModel user)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, user.type.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.username.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.full_name),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Sid, user.id.ToString())
            };
            return claims;
        }
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CustomerCreate([FromBody] Customercreate model)
        {
            var validator = ValitRules<Customercreate>
                      .Create()
                        .Ensure(m => m.name, rule => rule.Required())
                       .Ensure(m => m.phone, rule => rule.Required())
                          .For(model)
                           .Validate();
            try
            {
                var Customer = await this._customerRepository.CustomerCreate(model);
                return Ok(new ResponseSingleContentModel<Customercreate>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = Customer
                });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }

        //  [Authorize(Roles = "CUSTOMERDETAIL")]
        [AllowAnonymous]
        [HttpGet("detail")]
        public async Task<IActionResult> Customer(long id)
        {
            try
            {
                var Customer = await this._customerRepository.Customer(id);
                return Ok(new ResponseSingleContentModel<CustomerModel>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = Customer
                });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        //  [Authorize(Roles = "CUSTOMERLIST")]
        [AllowAnonymous]
        [HttpPost("list")]
        public async Task<IActionResult> CustomerList([FromBody] CustomerSearch model)
        {
            try
            {
                var Customer = await this._customerRepository.CustomerList(model);
                return Ok(new ResponseSingleContentModel<PaginationSet<CustomerModel>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = Customer
                });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        [HttpPost("modify")]
        public async Task<IActionResult> CustomerModify([FromBody] CustomerModel model)
        {
            try
            {
                model.userUpdated = userid(_httpContextAccessor);
                var Customer = await this._customerRepository.CustomerModify(model);
                return Ok(new ResponseSingleContentModel<CustomerModel>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = Customer
                });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        // [Authorize(Roles = "CUSTOMERDELETE")]
        [AllowAnonymous]
        [HttpDelete("delete")]
        public async Task<IActionResult> CustomerDelete(long customer_id)
        {
            try
            {
                long user_id = userid(_httpContextAccessor);
                var response = await this._customerRepository.CustomerDelete(customer_id, user_id);
                if (response)
                {
                    return Ok(new ResponseSingleContentModel<bool>
                    {
                        StatusCode = 200,
                        Message = "Success",
                        Data = response
                    });
                }
                else
                    return this.RouteToInternalServerError();
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(CustomerLoginModel login)
        {
            try
            {
                var validator = ValitRules<CustomerLoginModel>
                    .Create()
                    .Ensure(m => m.username, rule => rule.Required())
                    .Ensure(m => m.password, rule => rule.Required())
                    .For(login)
                    .Validate();

                if (validator.Succeeded)
                {
                    var user = await _customerRepository.CustomerGetPhone(login.username,login.password);

                    if (user != null && user.id > 0)
                    {

                        try
                        {

                            CustomerModel customer = await _customerRepository.Customer(user.id);


                            return Ok(new ResponseSingleContentModel<CustomerModel>
                            {
                                StatusCode = 200,
                                Message = "Đăng nhập thành công",
                                Data = customer
                            });
                        }
                        catch
                        {
                            return Ok(new ResponseSingleContentModel<string>
                            {
                                StatusCode = 500,
                                Message = "Sai tài khoản hoặc mật khẩu",
                                Data = null
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseSingleContentModel<string>
                        {
                            StatusCode = 500,
                            Message = "Tài khoản không tồn tại trong hệ thống",
                            Data = null
                        });
                    }
                }

                // Return invalidate data
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = string.Empty
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = string.Empty
                });
            }
        }
        [AllowAnonymous]
        [HttpPost("login-facebook")]
        public async Task<IActionResult> LocginFacebook(FaceebookUserRequest request)
        {
            var validator = ValitRules<FaceebookUserRequest>
                  .Create()
                  .Ensure(m => m.IdToken, rule => rule.Required())
                  .For(request)
                  .Validate();
            try
            {
                var item = await _customerRepository.Locginfacebook(request);
                if (item != null)
                {
                    return Ok(new ResponseSingleContentModel<CustomerModel>
                    {
                        StatusCode = 200,
                        Message = "đăng nhập thành công",
                        Data = item
                    }
                       );
                }
                else
                {
                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = "có lỗi trong quá trình sử lý",
                        Data = null

                    });
                }
            }
            catch
            {
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 500,
                    Message = "có lỗi trong quá trình sử lý",
                    Data = null

                });
            }
        }

        [AllowAnonymous]
        [HttpPost("locgin-goole")]
        public async Task<IActionResult> LocginGoogle([FromBody] GoogleUserRequest request)
        {
            var validator = ValitRules<GoogleUserRequest>
                   .Create()
                   .Ensure(m => m.IdToken, rule => rule.Required())
                   .For(request)
                   .Validate();
            if (validator.Succeeded)
            {
                try
                {
                    CustomerModel item = await _customerRepository.Locgingoogle(request);

                    if (item != null)
                    {

                        return Ok(new ResponseSingleContentModel<CustomerModel>
                        {
                            StatusCode = 500,
                            Message = "đăng nhập thành công",
                            Data = item
                        }); ;
                    }
                    else
                    {
                        return Ok(new ResponseSingleContentModel<string>
                        {
                            StatusCode = 500,
                            Message = " Có lỗi xảy ra trong quá trình xử lý",
                            Data = null
                        });
                    }
                }
                catch
                {
                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = " Có lỗi xảy ra trong quá trình xử lý",
                        Data = null
                    });
                }
            }
            else
            {
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 500,
                    Message = " Có lỗi xảy ra trong quá trình xử lý",
                    Data = null
                });
            }

        }
    }
}