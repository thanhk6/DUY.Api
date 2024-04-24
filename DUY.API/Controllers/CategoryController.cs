using C.Tracking.API.Entities;
using C.Tracking.API.Extensions;
using C.Tracking.API.IRepositories;
using C.Tracking.API.Model;
using C.Tracking.API.Model.Category;
using C.Tracking.Framework.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace C.Tracking.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryController(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        #region ratio

        //  [Authorize(Roles = "CATEGORYRATIODETAIL")]
        [HttpGet("category-ratio")]
        public async Task<IActionResult> CategoryRatio(long id)
        {
            try
            {
                var categorys = await this._categoryRepository.CategoryRatio(id);
                return Ok(new ResponseSingleContentModel<Category_Ratio>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = categorys
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
        //   [Authorize(Roles = "CATEGORYRATIOCREATE")]

        [HttpPost("category-ratio-create")]
        public async Task<IActionResult> CategoryRatioCreate([FromBody] Category_Ratio model)
        {
            try
            {
                var validator = ValitRules<Category_Ratio>
                    .Create()
                    .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    model.userAdded = userid(_httpContextAccessor);
                    var category = await this._categoryRepository.CategoryRatioCreate(model);
                    return Ok(new ResponseSingleContentModel<Category_Ratio>
                    {
                        StatusCode = 200,
                        Message = "Thêm mới thành công",
                        Data = category
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

        //   [Authorize(Roles = "CATEGORYRATIOMODIFY")]
        [HttpPost("category-ratio-modify")]
        public async Task<IActionResult> CategorycategoryModify([FromBody] Category_Ratio model)
        {
            try
            {
                var validator = ValitRules<Category_Ratio>
                    .Create()
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    model.userUpdated = userid(_httpContextAccessor);

                    var category = await this._categoryRepository.CategoryRatioModify(model);

                    return Ok(new ResponseSingleContentModel<Category_Ratio>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = category
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

        //   [Authorize(Roles = "CATEGORYRATIOLIST")]
        [HttpGet("category-ratio-list")]
        public async Task<IActionResult> CategoryRatioList(string? keyword, int page_size, int page_number)
        {
            try
            {
                var categorys = await this._categoryRepository.CategoryRatioList(keyword, page_size, page_number);
                return Ok(new ResponseSingleContentModel<PaginationSet<Category_Ratio>>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = categorys
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

        //   [Authorize(Roles = "CATEGORYRATIODELETE")]
        [HttpDelete("category-ratio-delete")]
        public async Task<IActionResult> CategoryratioDelete(long category_id)
        {
            try
            {
                long user_id = userid(_httpContextAccessor);

                var response = await this._categoryRepository.CategoryRatioDelete(category_id, user_id);
                if (response)
                {
                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = ""
                    });
                }
                else

                    return Ok(new ResponseSingleContentModel<IResponseData>
                    {
                        StatusCode = 500,
                        Message = "Không tìm thấy bản ghi",
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

        #region store
        //   [Authorize(Roles = "CATEGORYSTOREDETAIL")]
        [HttpGet("category-store")]
        public async Task<IActionResult> CategoryStore(long id)
        {
            try
            {
                var store = await this._categoryRepository.CategoryStore(id);
                return Ok(new ResponseSingleContentModel<Category_StoreModel>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = store
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

        // [Authorize(Roles = "CATEGORYSTORECREATE")]
        [HttpPost("category-store-create")]
        public async Task<IActionResult> CategoryStoreCreate([FromBody] Category_StoreModel model)
        {
            try
            {
                var validator = ValitRules<Category_StoreModel>
                    .Create()
                    .For(model)
                    .Validate();
                if (validator.Succeeded)
                {
                    model.userAdded = userid(_httpContextAccessor);
                    var store = await this._categoryRepository.CategoryStoreCreate(model);
                    return Ok(new ResponseSingleContentModel<Category_StoreModel>
                    {
                        StatusCode = 200,
                        Message = "Thêm mới thành công",
                        Data = store
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

        //   [Authorize(Roles = "CATEGORYSTOREMODIFY")]
        [HttpPost("category-store-modify")]
        public async Task<IActionResult> CategoryStoreModify([FromBody] Category_StoreModel model)
        {
            try
            {
                var validator = ValitRules<Category_StoreModel>
                    .Create()
                    .For(model)
                    .Validate();

                if (validator.Succeeded)
                {
                    model.userUpdated = userid(_httpContextAccessor);

                    var store = await this._categoryRepository.CategoryStoreModify(model);

                    return Ok(new ResponseSingleContentModel<Category_StoreModel>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = store
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
        //[AllowAnonymous]

        // [Authorize(Roles = "CATEGORYSTORELIST")]
        [HttpGet("category-store-list")]
        public async Task<IActionResult> CategoryStoreList(string? keyword, int? status, int page_size, int page_number, int? type)
        {
            try
            {
                var stores = await this._categoryRepository.CategoryStoreList(keyword, status, page_size, page_number, type);
                return Ok(new ResponseSingleContentModel<PaginationSet<Category_StoreViewModel>>
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công.",
                    Data = stores
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseSingleContentModel<IResponseData>
                {
                    StatusCode = 500,
                    Message = "Có lỗi trong quá trình xử lý",
                    Data = null
                });
            }
        }

        // [Authorize(Roles = "CATEGORYSTOREDELETE")]
        [HttpDelete("category-store-delete")]
        public async Task<IActionResult> CategoryStoreDelete(long category_id)
        {
            try
            {
                long user_id = userid(_httpContextAccessor);

                var response = await this._categoryRepository.CategoryStoreDelete(category_id, user_id);
                if (response)
                {
                    return Ok(new ResponseSingleContentModel<string>
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công",
                        Data = ""
                    });
                }
                else

                    return Ok(new ResponseSingleContentModel<IResponseData>
                    {
                        StatusCode = 500,
                        Message = "Không tìm thấy bản ghi",
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
