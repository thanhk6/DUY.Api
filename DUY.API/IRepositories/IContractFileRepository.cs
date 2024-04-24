
using DUY.API.Entities;


namespace DUY.API.IRepositories
{
    public interface IContractFileRepository
    {
        void FileCreate(List< Entities.File> file, string table_name, long id, byte type = 1);
        void FileModify(List<Entities.File> file, string table_name, long id, byte type = 1);
        void FileSingleCreate(Entities.File file, string table_name, long id, byte type);
        void FileSingleModify(Entities.File file, string table_name, long id, byte type);

        Task<List<Entities.File>> FileList(string table_name, long id);
    }
}
