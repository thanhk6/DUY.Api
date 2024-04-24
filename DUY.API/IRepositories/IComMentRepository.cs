using C.Tracking.API.Model;

using DUY.API.Model.ComMent;
using DUY.API.Model.Contract;

namespace C.Tracking.API.IRepositories
{
    public interface IComMentRepository
    {
        Task<ComMentModel> ContractCreate(ComMentModel model);
        Task<ComMentModel> ContractUpdate(ComMentModel model);
        Task<bool> ContractDelete(long contract_id, long user_id);
        Task<ComMentModel> ContractGetById(long id);
        Task<PaginationSet<ComMentModel>> ContractList(CommentSearch model);
       
    }
}
