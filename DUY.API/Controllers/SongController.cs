
using C.Tracking.API.Model;
using C.Tracking.Framework.Validator;
using DUY.API.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DUY.API.Model.Song;
using C.Tracking.API.Extensions;
using DUY.API.Model.Contract;

namespace DUY.API.Controllers
{
    [Route("api/song")]
    [ApiController]
    public class SongController : BaseController
    {


        private readonly IConfiguration _configuration;
        private readonly ISongRepository _songRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SongController(ISongRepository SongRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {

            _songRepository = SongRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

        }
        //  [Authorize(Roles = "CONTRACTCREATE")]
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> SongCreate([FromBody] Songmodel model)
        {
            try
            {
                var validator = ValitRules<Songmodel>
                    .Create()
                   //  .Ensure(m => m.customer_id, rule => rule.Required())
                   // .Ensure(m => m.customer_phone, rule => rule.Required())
                   //.Ensure(m => m.password, rule => rule.Required())
                   .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    //  model.userAdded = userid(_httpContextAccessor);
                    var response = await this._songRepository.SongCreate(model);
                    return response != null
                        ? Ok(new ResponseSingleContentModel<Songmodel>
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
        public async Task<IActionResult> SongtUpdate([FromBody] Songmodel model)
        {
            try
            {
                var validator = ValitRules<Songmodel>
                    .Create()
                    //.Ensure(m => m.name, rule => rule.Required())
                    //.Ensure(m => m.phone, rule => rule.Required())
                    //.Ensure(m => m.password, rule => rule.Required())
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    var userUpdated = userid(_httpContextAccessor);
                    var response = await this._songRepository.SongModify(model);
                    return response != null
                        ? Ok(new ResponseSingleContentModel<Songmodel>
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
        public async Task<IActionResult> SongGetById(long id)
        {
            try
            {
                var response = await this._songRepository.SongGetid(id);
                return response != null
                    ? Ok(new ResponseSingleContentModel<SongComment>
                    {
                        StatusCode = 200,
                        Message = "Lấy thông tin thành công",
                        Data = response
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<Songmodel>
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
        public async Task<IActionResult> SongList(string? keyword, int page_size, int page_number)
        {
            try
            {
                var response = await this._songRepository.SongList(keyword, page_size, page_number);
                return response != null
                    ? Ok(new ResponseSingleContentModel<PaginationSet<Songmodel>>
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
        public async Task<IActionResult> SongDelete(long id)
        {
            try
            {
                long user_id = userid(_httpContextAccessor);
                var response = await this._songRepository.SongDelete(id, user_id);
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



        [AllowAnonymous]
        [HttpPost("toolupaload")]
        public async Task<IActionResult> ToolUpload()
        {
            try
            {
                var response = await this._songRepository.toolupload();
                return response == true
                    ? Ok(new ResponseSingleContentModel<bool>
                    {
                        StatusCode = 200,
                        Message = "cập nhật thành công",
                        Data = response
                    })
                    : (IActionResult)Ok(new ResponseSingleContentModel<bool>
                    {
                        StatusCode = 500,
                        Message = "có lỗi trong quá trình upload ",
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
   
