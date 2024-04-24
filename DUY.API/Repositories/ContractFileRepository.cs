using DUY.API.Entities;

using Microsoft.EntityFrameworkCore;
using DUY.API.IRepositories;

namespace DUY.API.Repositories
{
    public class ContractFileRepository : IContractFileRepository
    {
        private readonly ApplicationContext _context;


        public ContractFileRepository(ApplicationContext context)
        {
            _context = context;
        }
        public void FileCreate(List<Entities.File> file, string table_name, long id, byte type)
        {
            foreach (var item in file)
            {
                item.tablename = table_name;
                item.idtable = id;
                item.type = type;
            }
            _context.Files.AddRange(file);
            _context.SaveChanges();
        }

        public void FileModify(List<Entities.File> file, string table_name, long id, byte type)
        {
            foreach (var item in file)
            {
                if (item.id == 0)
                {
                    item.tablename = table_name;
                    item.idtable = id;
                    item.type = type;
                    _context.Files.Add(item);
                }
                else
                {
                    _context.Entry(item).State = EntityState.Modified;

                }

            }
            _context.SaveChanges();
        }
        public void FileSingleCreate(Entities.File file, string table_name, long id, byte type)
        {

            file.tablename = table_name;
            file.idtable = id;
            file.type = type;

            _context.Files.Add(file);
            _context.SaveChanges();
        }
        public void FileSingleModify(Entities.File file, string table_name, long id, byte type)
        {
            if (file.id == 0)
            {
                file.tablename = table_name;
                file.idtable = id;
                file.type = type;

                _context.Files.Add(file);
            }
            else
            {
                file.type = type;
                _context.Entry(file).State = EntityState.Modified;

            }

            _context.SaveChanges();


        }
        public async Task<List<Entities.File>> FileList(string table_name, long id)
        {
            await Task.CompletedTask;
            List<Entities.File> file = new List<Entities.File>();
            file = _context.Files.Where(x => x.tablename == table_name && x.idtable == id && !x.is_delete).ToList();
            return file;
        }

    }
}
