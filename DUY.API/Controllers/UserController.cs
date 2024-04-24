using C.Tracking.API.Entities;
using C.Tracking.API.Extensions;
using C.Tracking.API.Model;
using C.Tracking.API.Model.User;
using C.Tracking.API.Repositories;
using C.Tracking.Framework.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using C.Tracking.API.IRepositories;

namespace C.Tracking.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserRepository userRepository, IAdminRepository adminRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }



        #region user
        [AllowAnonymous]
        [HttpPost("admin-user-create")]
        public async Task<IActionResult> UserCreate([FromBody] UserCreateModel model)
        {
            try
            {
                var checkUser = await this._userRepository.CheckUserExists(model.username, model.phone_number, model.email);
                if (checkUser > 0)
                {
                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 400,
                        Message = "Tài khoản, email hoặc số điện thoại đã được đăng ký vui lòng kiểm tra lại",
                        Data = string.Empty
                    });
                }
                var validator = ValitRules<UserCreateModel>
                    .Create()
                    .Ensure(m => m.full_name, rule => rule.Required())
                    .Ensure(m => m.email, rule => rule.Required())
                    .Ensure(m => m.phone_number, rule => rule.Required())
                    .Ensure(m => m.address, rule => rule.Required())
                    .Ensure(m => m.username, rule => rule.Required())
                    .Ensure(m => m.code, rule => rule.Required())
                    .Ensure(m => m.password, rule => rule.Required())
                    .Ensure(m => m.province_id, rule => rule.IsGreaterThan(0))
                    .Ensure(m => m.district_id, rule => rule.IsGreaterThan(0))
                    .Ensure(m => m.ward_id, rule => rule.IsGreaterThan(0))
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    var user = await this._userRepository.UserCreate(model);

                    return Ok(new ResponseSingleContentModel<UserModel>
                    {
                        StatusCode = 200,
                        Message = "",
                        Data = user
                    });
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
                return this.RouteToInternalServerError();


            }
        }
        //  [Authorize(Roles = "ADMINUSERMODIFY")]
        [HttpPost("admin-user-modify")]
        public async Task<IActionResult> UserModify([FromBody] UserModifyModel userupdate)
        {
            try
            {
                var validator = ValitRules<UserModifyModel>
                    .Create()
                    .Ensure(m => m.username, rule => rule.Required())
                     .Ensure(m => m.code, rule => rule.Required())
                     .Ensure(m => m.id, rule => rule.IsGreaterThan(0))
                    .For(userupdate)
                    .Validate();
                if (validator.Succeeded)
                {
                    var user = await this._userRepository.UserModify(userupdate);

                    return Ok(new ResponseSingleContentModel<UserModifyModel>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = userupdate
                    });
                }
                // Return invalidate data
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
                });
            }
            catch (Exception exc)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }

        //  [Authorize(Roles = "ADMINUSERLIST")]
        [HttpGet("admin-user-list")]
        public async Task<IActionResult> UserList(string? full_name, string? username, int page_number = 0, int page_size = 20)
        {
            try
            {
                PaginationSet<UserModel> Data = await _userRepository.UserList(full_name, username, page_number, page_size);

                return Ok(new ResponseSingleContentModel<PaginationSet<UserModel>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = Data
                });
            }
            catch (Exception ex)
            {

                return Ok(new ResponseSingleContentModel<UserModel>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = new()
                });
            }

        }

        [HttpGet("admin-authorize-check")]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                long id = userid(_httpContextAccessor);
                var user = await _userRepository.UserGetById(id);


                return Ok(new ResponseSingleContentModel<UserModel>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = user
                });
            }
            catch (Exception ex)
            {

                return Ok(new ResponseSingleContentModel<UserModel>
                {
                    StatusCode = 500,
                    Message = "Đăng nhập không thành công " + ex.Message,
                    Data = new()
                });
            }
        }

        //  [Authorize(Roles = "ADMINUSERDETAIL")]
        [HttpGet("admin-user")]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                var user = await _userRepository.UserGetById(id);
                return Ok(new ResponseSingleContentModel<UserModel>
                {
                    StatusCode = 200,
                    Message = "Thêm mới thành công",
                    Data = user
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
        [HttpPost("admin-login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            try
            {
                var validator = ValitRules<LoginModel>
                    .Create()
                    .Ensure(m => m.username, rule => rule.Required())
                    .Ensure(m => m.password, rule => rule.Required())
                    .For(login)
                    .Validate();

                if (validator.Succeeded)
                {
                    var user = await _userRepository.CheckUser(login.username);
                    if (user != null)
                    {

                        var checkAccount = _userRepository.Authenticate(login);
                        UserTokenModel userAuthen = new();
                        if (checkAccount == 1)
                        {
                            List<string> roles = await _userRepository.GetRoleByUser(user.id);
                            ClaimModel claim = new ClaimModel
                            {
                                email = user.email,
                                full_name = user.full_name,
                                id = user.id,
                                type = user.type,
                                username = user.username,
                                roles = roles,
                            };
                            string tokenString = GenerateToken(claim);
                            userAuthen.token = tokenString;
                            userAuthen.id = user.id;

                            userAuthen.username = user.username;
                            userAuthen.full_name = user.full_name;
                            userAuthen.token = tokenString;
                            userAuthen.roles = roles;
                            UserModel usermodel = await _userRepository.UserGetById(user.id);
                            userAuthen.image = usermodel.image;
                            return Ok(new ResponseSingleContentModel<UserTokenModel>
                            {
                                StatusCode = 200,
                                Message = "Đăng nhập thành công",
                                Data = userAuthen
                            });
                        }
                        else
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
        // [Authorize(Roles = "QUANLYNGUOIDUNG")]

        // [Authorize(Roles = "ADMINUSERCHANGEPASS")]
        [HttpPost("admin-user-changepass")]
        public async Task<IActionResult> ChangePassUser(ChangePassModel model)
        {
            try
            {
                var validator = ValitRules<ChangePassModel>
                    .Create()
                    .Ensure(m => m.passwordNew, rule => rule.Required())
                    .Ensure(m => m.passwordOld, rule => rule.Required())
                    .Ensure(m => m.id, rule => rule.IsGreaterThan(0))
                    .For(model)
                    .Validate();
                //if (!validator.Succeeded)   return  new ResponseSingleContentModel<object>
                //           {
                //               StatusCode = 400,
                //               Message = validator.ErrorMessages.JoinNewLine(),
                //                Data = new {validator.ErrorCodes,validator.ErrorMessages }
                //           }.Result;

                if (validator.Succeeded)
                {
                    var user = await this._userRepository.ChangePassUser(model);

                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Thêm mới thành công",
                        Data = null
                    });
                }

                // Return invalidate data
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
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
                // return this.RouteToInternalServerError();
            }
        }
        private string GenerateToken(ClaimModel user)
        {
            var identity = GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenSettings:Key"]));
            var token = new JwtSecurityToken(
            _configuration["TokenSettings:Issuer"],
             _configuration["TokenSettings:Audience"],
              expires: DateTime.Now.AddMonths(3),
              claims: identity,
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
              );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private IEnumerable<Claim> GetClaims(ClaimModel user)
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

            foreach (var userRole in user.roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return claims;
        }
        #endregion

        #region admingroup
        //  [Authorize(Roles = "ADMINGROUPCREATE")]
        [HttpPost("admin-group-create")]
        public async Task<IActionResult> AdminGroupCreate([FromBody] Admin_Group model)
        {
            try
            {
                var validator = ValitRules<Admin_Group>
                    .Create()
                    .Ensure(m => m.name, rule => rule.Required())
                     .Ensure(m => m.code, rule => rule.Required())
                    .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    model.userAdded = userid(_httpContextAccessor);
                    var group = await this._adminRepository.GroupCreate(model);
                    return Ok(new ResponseSingleContentModel<Admin_Group>
                    {
                        StatusCode = 200,
                        Message = "Thêm mới thành công",
                        Data = group
                    });
                }
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //   [Authorize(Roles = "ADMINGROUPMODIFY")]
        [HttpPost("admin-group-modify")]
        public async Task<IActionResult> AdminGroupModify([FromBody] Admin_Group model)
        {
            try
            {
                var validator = ValitRules<Admin_Group>
                    .Create()
                    .Ensure(m => m.name, rule => rule.Required())
                     .Ensure(m => m.code, rule => rule.Required())
                    .Ensure(m => m.id, rule => rule.IsGreaterThan(0))
                    .Ensure(m => m.note, rule => rule.Required())
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    var group = await this._adminRepository.GroupModify(model);

                    return Ok(new ResponseSingleContentModel<Admin_Group>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = group
                    });
                }

                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        // [Authorize(Roles = "ADMINGROUPLIST")]
        [HttpGet("admin-group-list")]
        public async Task<IActionResult> AdminGroupList(long id)
        {
            try
            {
                var group = await this._adminRepository.GroupList(id);
                return Ok(new ResponseSingleContentModel<List<Admin_Group>>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = group
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINGROUPDELETE")]
        [HttpDelete("admin-group-delete")]
        public async Task<IActionResult> AdminGroupDelete(long id)
        {
            try
            {
                var group = await this._adminRepository.GroupDelete(id);
                return group
                    ? Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Xóa bản ghi thành công",
                        Data = null
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = "Bản ghi không tồn tại hoặc bị xóa trước đó",
                        Data = null
                    });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        #endregion

        #region adminGroupUser
        // [Authorize(Roles = "ADMINGROUPUSERLIST")]
        [HttpGet("admin-group-user-list")]
        public async Task<IActionResult> AdminGroupUserList(long user_id)
        {
            try
            {
                var groupuser = await this._adminRepository.GroupUserList(user_id);

                return Ok(new ResponseSingleContentModel<List<Admin_Group_User>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = groupuser
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINGROUPUSERLISTBYGROUPID")]
        [HttpGet("admin-group-user-list-by-group-id")]
        public async Task<IActionResult> AdminGroupUserListByGroupId(long group_id)
        {
            try
            {
                var groupuser = await this._adminRepository.GroupUserListByGroupId(group_id);

                return Ok(new ResponseSingleContentModel<List<Admin_Group_User>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = groupuser
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = null
                });
            }
        }
        // [Authorize(Roles = "ADMINGROUPUSERCREATELIST")]
        [HttpPost("admin-group-user-create-list")]
        public async Task<IActionResult> AdminGroupUserCreateList([FromBody] List<Admin_Group_User> model)
        {
            try
            {
                var userAdded = userid(_httpContextAccessor);
                var groupusers = await this._adminRepository.GroupUserCreateList(model, userAdded);

                return Ok(new ResponseSingleContentModel<List<Admin_Group_User>>
                {
                    StatusCode = 200,
                    Message = "Thêm mới thành công",
                    Data = groupusers
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINGROUPUSERMODIFYLIST")]
        [HttpPost("admin-group-user-modify-list")]
        public async Task<IActionResult> AdminGroupUserModifyList([FromBody] List<Admin_Group_User> model)
        {
            try
            {
                var userModify = userid(_httpContextAccessor);
                var groupusers = await this._adminRepository.GroupUserModifyList(model, userModify);

                return Ok(new ResponseSingleContentModel<List<Admin_Group_User>>
                {
                    StatusCode = 200,
                    Message = "Cập nhật thành công",
                    Data = groupusers
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        // [Authorize(Roles = "ADMINGROUPUSERDELETE")]
        [HttpDelete("admin-group-user-delete")]
        public async Task<IActionResult> AdminGroupUserDelete(long id)
        {
            try
            {
                var delete = await this._adminRepository.GroupUserDelete(id);
                return delete == "0"
                    ? Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Success",
                        Data = null
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = "Xóa bản ghi không thành công " + delete,
                        Data = null
                    });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        #endregion

        #region adminRole
        //   [Authorize(Roles = "ADMINROLECREATE")]
        [HttpPost("admin-role-create")]
        public async Task<IActionResult> AdminRoleCreate([FromBody] Admin_Role model)
        {
            try
            {
                var validator = ValitRules<Admin_Role>
                    .Create()
                    .Ensure(m => m.name, rule => rule.Required())
                     .Ensure(m => m.code, rule => rule.Required())
                    .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    model.userAdded = userid(_httpContextAccessor);
                    var role = await this._adminRepository.RoleCreate(model);
                    return Ok(new ResponseSingleContentModel<Admin_Role>
                    {
                        StatusCode = 200,
                        Message = "Thêm mới thành công",
                        Data = role
                    });
                }
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINROLEMODIFY")]
        [HttpPost("admin-role-modify")]
        public async Task<IActionResult> AdminRoleModify([FromBody] Admin_Role model)
        {
            try
            {
                var validator = ValitRules<Admin_Role>
                    .Create()
                    .Ensure(m => m.name, rule => rule.Required())
                     .Ensure(m => m.code, rule => rule.Required())
                    .Ensure(m => m.id, rule => rule.IsGreaterThan(0))
                    .Ensure(m => m.note, rule => rule.Required())
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    var role = await this._adminRepository.RoleModify(model);

                    return Ok(new ResponseSingleContentModel<Admin_Role>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = role
                    });
                }
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = null
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINROLELIST")]
        [HttpGet("admin-role-list")]
        public async Task<IActionResult> AdminRoleList(long id)
        {
            try
            {
                var role = await this._adminRepository.RoleList(id);
                return Ok(new ResponseSingleContentModel<List<Admin_Role>>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = role
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINROLEDELETE")]
        [HttpDelete("admin-role-delete")]
        public async Task<IActionResult> AdminRoleDelete(long id)
        {
            try
            {
                var role = await this._adminRepository.RoleDelete(id);
                return role
                    ? Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Xóa bản ghi thành công",
                        Data = null
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = "Bản ghi không tồn tại hoặc bị xóa trước đó",
                        Data = null
                    });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        #endregion

        #region adminRoleGroup
        // [Authorize(Roles = "ADMINROLEGROUPLIST")]
        [HttpGet("admin-role-group-list")]
        public async Task<IActionResult> AdminRoleGroupList(long role_id)
        {
            try
            {
                var rolegroups = await this._adminRepository.RoleGroupList(role_id);

                return Ok(new ResponseSingleContentModel<List<Admin_Role_Group>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = rolegroups
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINROLEGROUPLISTBYGROUPID")]
        [HttpGet("admin-role-group-list-by-group-id")]
        public async Task<IActionResult> AdminRoleGroupListByGroupId(long group_id)
        {
            try
            {
                var rolegroups = await this._adminRepository.RoleGroupListByGroupId(group_id);

                return Ok(new ResponseSingleContentModel<List<Admin_Role_Group>>
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = rolegroups
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = null
                });
            }
        }
        // [Authorize(Roles = "ADMINROLEGROUPCREATELIST")]
        [HttpPost("admin-role-group-create-list")]
        public async Task<IActionResult> AdminRoleGroupCreateList([FromBody] List<Admin_Role_Group> model)
        {
            try
            {
                var userAdded = userid(_httpContextAccessor);
                var rolegroups = await this._adminRepository.RoleGroupCreateList(model, userAdded);

                return Ok(new ResponseSingleContentModel<List<Admin_Role_Group>>
                {
                    StatusCode = 200,
                    Message = "Thêm mới thành công",
                    Data = rolegroups
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //  [Authorize(Roles = "ADMINROLEGROUPMODIFYLIST")]
        [HttpPost("admin-role-group-modify-list")]
        public async Task<IActionResult> AdminRoleGroupModifyList([FromBody] List<Admin_Role_Group> model)
        {
            try
            {
                var userModify = userid(_httpContextAccessor);
                var rolegroups = await this._adminRepository.RoleGroupModifyList(model, userModify);

                return Ok(new ResponseSingleContentModel<List<Admin_Role_Group>>
                {
                    StatusCode = 200,
                    Message = "Cập nhật thành công",
                    Data = rolegroups
                });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        //   [Authorize(Roles = "ADMINROLEGROUPDELETE")]
        [HttpDelete("admin-role-group-delete")]
        public async Task<IActionResult> AdminRoleGroupDelete(long id)
        {
            try
            {
                var delete = await this._adminRepository.RoleGroupDelete(id);
                return delete == "0"
                    ? Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Success",
                        Data = null
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 500,
                        Message = "Xóa bản ghi không thành công " + delete,
                        Data = null
                    });
            }
            catch (Exception)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }
        #endregion
    }
}
