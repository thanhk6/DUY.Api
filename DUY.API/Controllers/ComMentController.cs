using C.Tracking.API.Extensions;
using C.Tracking.API.IRepositories;
using C.Tracking.API.Model;

using C.Tracking.Framework.Validator;
using DUY.API.Model.ComMent;
using DUY.API.Model.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DUY.API.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class ComMentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IComMentRepository _contractRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ComMentController(IComMentRepository contractRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _contractRepository = contractRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        //  [Authorize(Roles = "CONTRACTCREATE")]
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> ComMentCreate([FromBody] ComMentModel model)
        {
            try
            {
                var validator = ValitRules<ComMentModel>
                    .Create()
                   //  .Ensure(m => m.customer_id, rule => rule.Required())
                   // .Ensure(m => m.customer_phone, rule => rule.Required())
                   //.Ensure(m => m.password, rule => rule.Required())
                   .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    //  model.userAdded = userid(_httpContextAccessor);
                    var response = await this._contractRepository.ContractCreate(model);
                    return response != null
                        ? Ok(new ResponseSingleContentModel<ComMentModel>
                        {
                            StatusCode = 200,
                            Message = "Thêm mới thành công",
                            Data = response
                        })
                        : (IActionResult)Ok(new ResponseSingleContentModel<string>
                        {
                            StatusCode = 500,
                            Message = "Có lỗi trong quá trình xử lý ",
                            Data = string.Empty
                        });
                }
                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = string.Empty
                });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        //  [Authorize(Roles = "CONTRACTMODIFY")]
        [AllowAnonymous]
        [HttpPost("modify")]
        public async Task<IActionResult> ComMentUpdate([FromBody] ComMentModel model)
        {
            try
            {
                var validator = ValitRules<ComMentModel>
                    .Create()
                    //.Ensure(m => m.name, rule => rule.Required())
                    //.Ensure(m => m.phone, rule => rule.Required())
                    //.Ensure(m => m.password, rule => rule.Required())
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    var  userUpdated = userid(_httpContextAccessor);
                    var response = await this._contractRepository.ContractUpdate(model);
                    return response != null
                        ? Ok(new ResponseSingleContentModel<ComMentModel>
                        {
                            StatusCode = 200,
                            Message = "Cập nhật thành công",
                            Data = response
                        })
                        : (IActionResult)Ok(new ResponseSingleContentModel<string>
                        {
                            StatusCode = 500,
                            Message = "Có lỗi trong quá trình xử lý ",
                            Data = string.Empty
                        });
                }

                return Ok(new ResponseSingleContentModel<string>
                {
                    StatusCode = 400,
                    Message = validator.ErrorMessages.JoinNewLine(),
                    Data = string.Empty
                });
            }

            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        //  [Authorize(Roles = "CONTRACTCHANGESTATUS")]
        [HttpGet("detail")]
        public async Task<IActionResult> ComMentGetById(long id)
        {
            try
            {
                var response = await this._contractRepository.ContractGetById(id);
                return response != null
                    ? Ok(new ResponseSingleContentModel<ComMentModel>
                    {
                        StatusCode = 200,
                        Message = "Lấy thông tin thành công",
                        Data = response
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<ComMentModel>
                    {
                        StatusCode = 500,
                        Message = "Không tìm thấy bản ghi ",
                        Data = null
                    });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        // [Authorize(Roles = "CONTRACTLIST")]
        [AllowAnonymous]
        [HttpPost("list")]
        public async Task<IActionResult> ContractList(CommentSearch model)
        {
            try
            {
                var response = await this._contractRepository.ContractList( model);
                return response != null
                    ? Ok(new ResponseSingleContentModel<PaginationSet<ComMentModel>>
                    {
                        StatusCode = 200,
                        Message = "Lấy thông tin thành công",
                        Data = response
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<PaginationSet<ComMentModel>>
                    {
                        StatusCode = 500,
                        Message = "Có lỗi trong quá trình xử lý ",
                        Data = new PaginationSet<ComMentModel>()
                    });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        //  [Authorize(Roles = "CONTRACTDELETE")]
        [AllowAnonymous]
        [HttpDelete("delete")]
        public async Task<IActionResult> ComMentDelete(long id)
        {
            try
            {
                long user_id = userid(_httpContextAccessor);
                var response = await this._contractRepository.ContractDelete(id, user_id);
                return response != true
                    ? Ok(new ResponseSingleContentModel<bool>
                    {
                        StatusCode = 200,
                        Message = "cập nhật thành công",
                        Data = response
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<bool>
                    {
                        StatusCode = 500,
                        Message = "Không tìm thấy bản ghi ",
                        Data = response
                    });
            }
            catch (Exception ex)
            {
                return this.RouteToInternalServerError();
            }
        }
        
      
    }
}
