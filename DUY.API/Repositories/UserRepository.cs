using DUY.API.Entities;
using C.Tracking.API.Extensions;
using C.Tracking.API.Model;
using C.Tracking.API.Model.User;
using AutoMapper;
using C.Tracking.API.IRepositories;
using C.Tracking.Extensions;
using Microsoft.EntityFrameworkCore;
using DUY.API.IRepositories;
using DUY.API.Model.User;

namespace DUY.API.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IContractFileRepository _file;

        public UserRepository(ApplicationContext context, IMapper mapper, IContractFileRepository fileRepository)
        {
            _mapper = mapper;
            _context = context;
            _file = fileRepository;
        }

        public async Task<UserModel> UserGetById(long id)
        {
            string tablename = Common.TableName.User.ToString();

            Admin_User user = _context.Admin_User.Find(id);
            UserModel userViewModel = _mapper.Map<UserModel>(user);
            userViewModel.image = _context.Files.Where(x => x.tablename == tablename && x.idtable == id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault();
            return userViewModel;
        }
        public async Task<UserModel> UserCreate(UserCreateModel useradd)
        {
            Admin_User user = _mapper.Map<Admin_User>(useradd);
            user.pass_code = Encryptor.RandomPassword();
            user.password = Encryptor.MD5Hash(user.password + user.pass_code);
            _context.Admin_User.Add(user);
            _context.SaveChanges();
            UserModel userViewModel = _mapper.Map<UserModel>(user);
            return userViewModel;
        }
        public async Task<UserModifyModel> UserModify(UserModifyModel userupdate)
        {
            var user = _context.Admin_User.FirstOrDefault(r => r.id == userupdate.id);

            user.address = userupdate.address;
            user.email = userupdate.email;
            user.phone_number = userupdate.phone_number;
            user.birthday = userupdate.birthday;
            user.district_id = userupdate.district_id;
            user.province_id = userupdate.province_id;
            user.ward_id = userupdate.ward_id;
            user.sex = userupdate.sex;
            user.type = userupdate.type;
            user.full_name = userupdate.full_name;
            user.is_active = userupdate.is_active;
            user.dateUpdated = DateTime.Now;

            //string tablename = Common.TableName.User.ToString();
            //var image = _context.Contract_File.AsNoTracking().Where(x => x.tablename == tablename && x.idtable == userupdate.id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault();
            //if (image == null)
            //    _file.FileSingleCreate(userupdate.image, tablename, userupdate.id, 1);
            //else
            //    _file.FileSingleModify(userupdate.image, tablename, userupdate.id, 1);


            UserModifyModel userViewModel = _mapper.Map<UserModifyModel>(user);

            _context.Admin_User.Update(user);
            _context.SaveChanges();
            return userupdate;
        }
        public async Task<bool> ChangePassUser(ChangePassModel model)
        {
            var user = _context.Admin_User.FirstOrDefault(r => r.id == model.id);
            LoginModel login = new LoginModel
            {
                password = model.passwordOld,
                username = user.username,
            };
            if (Authenticate(login) == 1)
            {
                user.dateUpdated = DateTime.Now;
                user.pass_code = Encryptor.RandomPassword();
                user.password = Encryptor.MD5Hash(model.passwordNew + user.pass_code);
                _context.Admin_User.Update(user);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<PaginationSet<UserModel>> UserList(string? full_name, string? username, int page_number, int page_size)
        {
            string tablename = Common.TableName.User.ToString();

            PaginationSet<UserModel> response = new PaginationSet<UserModel>();
            IEnumerable<UserModel> listItem = from a in _context.Admin_User
                                              select new UserModel
                                              {
                                                  address = a.address,
                                                  is_active = a.is_active,
                                                  sex = a.sex,
                                                  birthday = a.birthday,
                                                  code = a.code,
                                                  district_id = a.district_id,
                                                  email = a.email,
                                                  full_name = a.full_name,
                                                  id = a.id,
                                                  phone_number = a.phone_number,
                                                  province_id = a.province_id,
                                                  type = a.type,
                                                  userAdded = a.userAdded,
                                                  username = a.username,
                                                  ward_id = a.ward_id,
                                                  userUpdated = a.userUpdated,
                                                  image = _context.Files.Where(x => x.tablename == tablename && x.idtable == a.id && !x.is_delete).OrderByDescending(x => x.id).FirstOrDefault()

                                              };
            if (username != null && username != "")
            {
                listItem = listItem.Where(r => r.username.Contains(username));
            }
            if (full_name != null && full_name != "")
            {
                listItem = listItem.Where(r => r.full_name.Contains(full_name));
            }
            if (page_number > 0)
            {
                response.totalcount = listItem.Select(x => x.id).Count();
                response.page = page_number;
                response.maxpage = (int)Math.Ceiling((decimal)response.totalcount / page_size);
                response.lists = listItem.OrderByDescending(r => r.id).Skip(page_size * (page_number - 1)).Take(page_size).ToList();
            }
            else
            {
                response.lists = listItem.OrderByDescending(r => r.id).ToList();
            }
            return response;
        }
        public int Authenticate(LoginModel login)
        {
            Admin_User user = _context.Admin_User.Where(r => r.username.ToUpper() == login.username.ToUpper() || r.email.ToUpper() == login.username.ToUpper() || r.phone_number == login.username).FirstOrDefault();
            if (!user.is_active)
            {
                return -1;
            }
            else
            {
                var passWord = Encryptor.MD5Hash(login.password + user.pass_code);
                return passWord != user.password ? 2 : 1;
            }
        }
        public async Task<List<string>> GetRoleByUser(long user_id)
        {
            List<string> roles = new List<string>();
            roles = (from a in _context.Admin_Group_User
                     join b in _context.Admin_Group on a.group_id equals b.id
                     join c in _context.Admin_Role_Group on b.id equals c.group_id
                     join
                     d in _context.Admin_Role on c.role_id equals d.id
                     where a.user_id == user_id && !a.is_delete && !b.is_delete && !c.is_delete && !d.is_delete
                     select d.code).ToList();
            return roles;
        }
        public async Task<Admin_User> CheckUser(string username)
        {
            return _context.Admin_User.Where(r => r.username.ToUpper() == username.ToUpper() || r.email.ToUpper() == username.ToUpper() || r.phone_number == username).FirstOrDefault();

        }

        public async Task<int> CheckUserExists(string username, string phone_number, string email)
        {
            return _context.Admin_User.Where(r => r.username.ToUpper() == username.ToUpper() || r.email.ToUpper() == email.ToUpper() || r.phone_number == phone_number).Count();

        }
    }
}
