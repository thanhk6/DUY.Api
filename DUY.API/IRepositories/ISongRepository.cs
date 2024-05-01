using C.Tracking.API.Model.Customer;
using C.Tracking.API.Model;
using DUY.API.Model.Song;

namespace DUY.API.IRepositories
{
    public interface ISongRepository
    {
        Task<Songmodel>    SongCreate(Songmodel model);
        Task<SongComment> SongGetid(long id);

        Task<PaginationSet<Songmodel>> SongList(string? keyword ,int page_size, int page_number);
        Task<Songmodel> SongModify(Songmodel model);
        Task<bool> SongDelete(long song_id, long user_id);

        Task<bool> toolupload ();
    }
}
