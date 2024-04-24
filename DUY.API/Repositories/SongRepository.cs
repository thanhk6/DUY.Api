using AutoMapper;
using DUY.API.Entities;

using C.Tracking.API.Model;
using C.Tracking.Extensions;
using DUY.API.IRepositories;
using DUY.API.Model.Song;
using Microsoft.EntityFrameworkCore.Storage;
using File = DUY.API.Entities.File;

using Microsoft.EntityFrameworkCore;
using DUY.API.Model.Contract;


namespace DUY.API.Repositories
{
    public class SongRepository : ISongRepository

    {
        private readonly ApplicationContext _context;
        private IMapper _mapper;
        private IContractFileRepository _file;
        public SongRepository(ApplicationContext context, IMapper mapper, IContractFileRepository file)
        {
            _context = context;
            _mapper = mapper;
            _file = file;
        }
        public async Task<Songmodel> SongCreate(Songmodel model)
        {
            return await Task.Run(() =>
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Song item = _mapper.Map<Song>(model);

                        item.dateAdded = DateTime.Now;
                        string tablename = Common.TableName.Customer.ToString();
                        _context.Songs.Add(item);
                        _context.SaveChanges();
                        if(model.audio!=null)
                        {
                            File audio=model.audio;
                            _file.FileSingleCreate(audio, Common.TableName.Song.ToString(), item.id, 0);

                        }    

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
        public async Task<bool> SongDelete(long song_id, long user_id)
        {
            return await Task.Run(async () => {
          var Item =  await _context.Songs.FirstOrDefaultAsync(r=>r.id==song_id);

                if (Item == null) {
                    return false;               
                }
                else
                {
                    Item.userUpdated = user_id;
                    Item.dateUpdated= DateTime.Now;
                    Item.is_delete= true;
                    _context.Songs.Update(Item);
                }
                _context.SaveChanges();
                return true;
            } );
        }
        public  async Task<SongComment> SongGetid(long id)
        {
            string tablename = Common.TableName.Song.ToString();
          Song Item = await _context.Songs.Where(r => r.id == id && !r.is_delete).FirstOrDefaultAsync();

        List <ComMent> comMent = await _context.ComMents.Where(r => r.song_Id==Item.id).ToListAsync();



            SongComment model =  new SongComment();

            model.id = id;
            model.name = Item.name;
            model.description = Item.description;
            if (comMent != null)
            {
                model.comments = _mapper.Map<List<ComMentModel>>(comMent);
            }
            model.audio =  await _context.Files.Where(r => r.tablename == tablename && r.idtable == id && !r.is_delete).FirstOrDefaultAsync();
           
            return model;
        }

        public async Task<PaginationSet<Songmodel>> SongList(string? keyword,  int page_size, int page_number)
        {
            return await Task.Run(async () =>
            {
                string tablename = Common.TableName.Song.ToString();

                PaginationSet<Songmodel> response = new PaginationSet<Songmodel>();

                IQueryable<Songmodel> listItem = from a in _context.Songs
                                                 where !a.is_delete
                                                 select new Songmodel
                                                 {
                                                     id = a.id,
                                                     name = a.name,
                                                     description = a.description,
                                                     audio = _context.Files.Where(r => r.tablename == tablename && r.idtable == a.id).OrderBy(r => r.id).FirstOrDefault(),
                                                 };

                if (keyword is not null and not "")
                {
                    listItem = listItem.Where(r => r.name.Contains(keyword));
                }

                //if (status is not null)
                //    listItem = listItem.Where(r => r.status == status);

                //if (type is not null)
                //    listItem = listItem.Where(r => r.type == type);

                if (page_number > 0)
                {
                    response.totalcount = listItem.Select(x => x.id).Count();
                    response.page = page_number;
                    response.maxpage = (int)Math.Ceiling((decimal)response.totalcount / page_size);

                    response.lists = await listItem.OrderByDescending(r => r.id).Skip(page_size * (page_number - 1)).Take(page_size).ToListAsync();
                }

                response.lists = await listItem.ToListAsync();
                
                return response;

            });

        }

        public async Task<Songmodel> SongModify(Songmodel model)
        {
            return await Task.Run(async () =>
            {
                Song Item= _context.Songs.FirstOrDefault(r=>r.id==model.id);

                Item.dateUpdated = DateTime.Now;
                Item.name = model.name;
                Item.description = model.description;

            _context.Songs.Update(Item);
                _context.SaveChanges();
                if(model.audio!=null)
                {
                    File audio=model.audio;
                    _file.FileSingleModify(audio, Common.TableName.Song.ToString(), Item.id, 0);

                }    
                _context.Songs.Update(Item);
                _context.SaveChanges();
                model = _mapper.Map<Songmodel>(Item);
                return model;
            });
        }
    }
}
