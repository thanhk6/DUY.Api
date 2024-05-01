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
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Realms.Sync;
using Newtonsoft.Json.Linq;
using C.Tracking.API.Controllers;
using System.ComponentModel.DataAnnotations;


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

                        if (model.audio != null)
                        {
                            File audio = model.audio;
                            _file.FileSingleCreate(audio, Common.TableName.Song.ToString(), item.id, 0);

                        }
                        _context.SaveChanges();

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
            return await Task.Run(async () =>
            {
                var Item = await _context.Songs.FirstOrDefaultAsync(r => r.id == song_id);

                if (Item == null)
                {
                    return false;
                }
                else
                {
                    Item.userUpdated = user_id;
                    Item.dateUpdated = DateTime.Now;
                    Item.is_delete = true;
                    _context.Songs.Update(Item);
                }
                _context.SaveChanges();
                return true;
            });
        }
        public async Task<SongComment> SongGetid(long id)
        {
            string tablename = Common.TableName.Song.ToString();
            Song Item = await _context.Songs.Where(r => r.id == id && !r.is_delete).FirstOrDefaultAsync();

            List<ComMent> comMent = await _context.ComMents.Where(r => r.song_Id == Item.id).ToListAsync();



            SongComment model = new SongComment();

            model.id = id;
            model.name = Item.name;
            model.description = Item.description;
            if (comMent != null)
            {
                model.comments = _mapper.Map<List<ComMentModel>>(comMent);
            }
            model.audio = await _context.Files.Where(r => r.tablename == tablename && r.idtable == id && !r.is_delete).FirstOrDefaultAsync();

            return model;
        }
        public async Task<PaginationSet<Songmodel>> SongList(string? keyword, int page_size, int page_number)
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
                                                     author = a.author,
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
                Song Item = _context.Songs.FirstOrDefault(r => r.id == model.id);

                Item.dateUpdated = DateTime.Now;
                Item.name = model.name;
                Item.description = model.description;

                _context.Songs.Update(Item);
                _context.SaveChanges();
                if (model.audio != null)
                {
                    File audio = model.audio;
                    _file.FileSingleModify(audio, Common.TableName.Song.ToString(), Item.id, 0);

                }
                _context.Songs.Update(Item);
                _context.SaveChanges();
                model = _mapper.Map<Songmodel>(Item);
                return model;
            });
        }
        public async Task<bool> toolupload()
        {
            string path = @"D:\web\web api\duytin4\music\music\";
            string[] files = Directory.GetFiles(path);
            try
            {
                foreach (string item in files)
                {
                    string filePath = item;
                    if (!System.IO.File.Exists(filePath))
                    {
                        Console.WriteLine("File not found.");
                        return false;
                    }
                    Songmodel item1 = new Songmodel();
                    using (var httpClient = new HttpClient())
                    using (var content = new MultipartFormDataContent())
                    {
                        var fileStream = new FileStream(filePath, FileMode.Open);
                        var fileContent = new StreamContent(fileStream);
                        content.Add(fileContent, "file", Path.GetFileName(filePath));
                        string apiUrlSONG = "http://apiduy.homeled.vn/api/file/upload";
                        var response = await httpClient.PostAsync(apiUrlSONG, content);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            string responseData = await response.Content.ReadAsStringAsync();
                            JObject jsonObject = JObject.Parse(responseData);
                            JArray usersArray = (JArray)jsonObject["newFiles"];
                            List<FileModel> userList = usersArray.ToObject<List<FileModel>>();
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                            item1.id = 0;
                            item1.name = fileNameWithoutExtension;
                            item1.description = " tên bài hát  này là " + userList[0].name;
                            item1.author = " chưa cập nhập tên tác giả";
                            item1.audio = new File
                            {
                                name_guid = userList[0].name_guid,
                                name = userList[0].name,
                                type = userList[0].type,
                                path = userList[0].path,
                                file_type = userList[0].file_type,
                            };
                            await SongCreate(item1);
                            Console.WriteLine($"File uploaded successfully! Server response: {responseContent}");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to upload file. Status code: {response.StatusCode}");
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }



        }
    }
}
