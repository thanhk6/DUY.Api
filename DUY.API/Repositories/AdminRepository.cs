using DUY.API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using DUY.API.IRepositories;

namespace C.Tracking.API.Repositories
{
    internal class AdminRepository : IAdminRepository
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public AdminRepository(ApplicationContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        #region admingroup
        public async Task<Admin_Group> GroupCreate(Admin_Group model)
        {
            Admin_Group group = _mapper.Map<Admin_Group>(model);
            group.dateAdded = DateTime.Now;
            _context.Admin_Group.Add(group);
            _context.SaveChanges();

            model = _mapper.Map<Admin_Group>(group);
            return model;
        }
        public async Task<Admin_Group> GroupModify(Admin_Group model)
        {
            return await Task.Run(async () =>
            {
                Admin_Group group = await _context.Admin_Group.FirstOrDefaultAsync(r => r.id == model.id);
                group.note = model.note;
                group.code = model.code;
                group.name = model.name;
                group.dateUpdated = DateTime.Now;
                _context.Admin_Group.Update(group);

                _context.SaveChanges();
                return model;
            });
        }
        public async Task<List<Admin_Group>> GroupList(long id)
        {
            return await Task.Run(async () =>
            {
                List<Admin_Group> groups = new();
                groups = await (from a in _context.Admin_Group
                                where !a.is_delete
                                select new Admin_Group
                                {
                                    id = a.id,
                                    name = a.name,
                                    note = a.note,
                                    code = a.code,
                                }).ToListAsync();
                return groups;
            });
        }
        public async Task<bool> GroupDelete(long id)
        {
            try
            {
                Admin_Group group = await _context.Admin_Group.FirstOrDefaultAsync(r => r.id == id);
                group.is_delete = true;
                _context.Admin_Group.Update(group);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region adminGroupUser
        public async Task<List<Admin_Group_User>> GroupUserList(long user_id)
        {
            return await Task.Run(() =>

            {
                List<Admin_Group_User> admingroupusers = _context.Admin_Group_User.Where(r => r.user_id == user_id && !r.is_delete).ToList();
                var models = _mapper.Map<List<Admin_Group_User>>(admingroupusers);
                return models;
            });
        }
        public async Task<List<Admin_Group_User>> GroupUserListByGroupId(long group_id)
        {
            return await Task.Run(() =>
            {
                List<Admin_Group_User> admingroupusers = _context.Admin_Group_User.Where(r => r.group_id == group_id && !r.is_delete).ToList();
                var models = _mapper.Map<List<Admin_Group_User>>(admingroupusers);
                return models;
            });
        }

        public async Task<List<Admin_Group_User>> GroupUserCreateList(List<Admin_Group_User> models, long useradd)
        {
            return await Task.Run(() =>
              {
                  DateTime date = DateTime.Now;
                  List<Admin_Group_User> admingroupusers = _mapper.Map<List<Admin_Group_User>>(models);
                  foreach (var item in admingroupusers)
                  {
                      item.userAdded = useradd;
                      item.dateAdded = date;
                  }
                  _context.Admin_Group_User.AddRange(admingroupusers);
                  _context.SaveChanges();
                  models = _mapper.Map<List<Admin_Group_User>>(admingroupusers);
                  return models;
              });
        }

        public async Task<List<Admin_Group_User>> GroupUserModifyList(List<Admin_Group_User> models, long userModify)
        {
            return await Task.Run(() =>
            {
                DateTime date = DateTime.Now;

                List<Admin_Group_User> groupusers = new List<Admin_Group_User>();
                foreach (var item in models)
                {
                    Admin_Group_User group_User = new Admin_Group_User();
                    if (item.id == 0)
                    {
                        group_User = _mapper.Map<Admin_Group_User>(item);
                        group_User.userAdded = userModify;
                        group_User.dateAdded = date;
                        _context.Admin_Group_User.Add(group_User);
                    }
                    else
                    {
                        group_User = _context.Admin_Group_User.FirstOrDefault(r => r.id == item.id);
                        group_User.userUpdated = userModify;
                        group_User.dateUpdated = date;
                        group_User.user_id = item.user_id;
                        group_User.group_id = item.group_id;
                        group_User.is_delete = item.is_delete;
                        _context.Admin_Group_User.Update(group_User);
                    }
                    groupusers.Add(group_User);
                }
                _context.SaveChanges();
                models = _mapper.Map<List<Admin_Group_User>>(groupusers);
                return models;
            });
        }
        public async Task<string> GroupUserDelete(long user_id)
        {
            return await Task.Run(() =>
                  {
                      string mess = "0";
                      try
                      {
                          var admingroupuser = _context.Admin_Group_User.FirstOrDefault(r => r.user_id == user_id);
                          admingroupuser.is_delete = true;
                          _context.Admin_Group_User.Update(admingroupuser);
                          _context.SaveChanges();
                      }
                      catch (Exception ex)
                      {
                          mess = ex.Message;
                      }
                      return mess;
                  });
        }
        #endregion

        #region adminRole
        public async Task<Admin_Role> RoleCreate(Admin_Role model)
        {
            Admin_Role role = _mapper.Map<Admin_Role>(model);
            role.dateAdded = DateTime.Now;
            _context.Admin_Role.Add(role);
            _context.SaveChanges();

            model = _mapper.Map<Admin_Role>(role);
            return model;
        }
        public async Task<Admin_Role> RoleModify(Admin_Role model)
        {
            return await Task.Run(async () =>
            {
                Admin_Role role = await _context.Admin_Role.FirstOrDefaultAsync(r => r.id == model.id);
                role.note = model.note;
                role.code = model.code;
                role.name = model.name;
                role.dateUpdated = DateTime.Now;
                _context.Admin_Role.Update(role);

                _context.SaveChanges();
                return model;
            });
        }
        public async Task<List<Admin_Role>> RoleList(long id)
        {
            return await Task.Run(async () =>
            {
                List<Admin_Role> roles = new();
                roles = await (from b in _context.Admin_Role
                               where !b.is_delete
                               select new Admin_Role
                               {
                                   id = b.id,
                                   name = b.name,
                                   note = b.note,
                                   code = b.code,
                               }).ToListAsync();
                return roles;
            });
        }
        public async Task<bool> RoleDelete(long id)
        {
            try
            {
                Admin_Role role = await _context.Admin_Role.FirstOrDefaultAsync(r => r.id == id);
                role.is_delete = true;
                _context.Admin_Role.Update(role);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region adminRoleGroup
        public async Task<List<Admin_Role_Group>> RoleGroupList(long role_id)
        {
            return await Task.Run(() =>
            {
                List<Admin_Role_Group> adminrolegroups = _context.Admin_Role_Group.Where(r => r.role_id == role_id && !r.is_delete).ToList();
                var models = _mapper.Map<List<Admin_Role_Group>>(adminrolegroups);
                return models;
            });
        }
        public async Task<List<Admin_Role_Group>> RoleGroupListByGroupId(long group_id)
        {
            return await Task.Run(() =>
            {
                List<Admin_Role_Group> adminrolegroups = _context.Admin_Role_Group.Where(r => r.group_id == group_id && !r.is_delete).ToList();
                var models = _mapper.Map<List<Admin_Role_Group>>(adminrolegroups);
                return models;
            });
        }
        public async Task<List<Admin_Role_Group>> RoleGroupCreateList(List<Admin_Role_Group> models, long useradd)
        {
            return await Task.Run(() =>
            {
                DateTime date = DateTime.Now;
                List<Admin_Role_Group> adminrolegroups = _mapper.Map<List<Admin_Role_Group>>(models);
                foreach (var item in adminrolegroups)
                {
                    item.userAdded = useradd;
                    item.dateAdded = date;
                }
                _context.Admin_Role_Group.AddRange(adminrolegroups);
                _context.SaveChanges();
                models = _mapper.Map<List<Admin_Role_Group>>(adminrolegroups);
                return models;
            });
        }

        public async Task<List<Admin_Role_Group>> RoleGroupModifyList(List<Admin_Role_Group> models, long userModify)
        {
            return await Task.Run(() =>
            {
                DateTime date = DateTime.Now;

                List<Admin_Role_Group> rolegroups = new List<Admin_Role_Group>();
                foreach (var item in models)
                {
                    Admin_Role_Group role_Group = new Admin_Role_Group();
                    if (item.id == 0)
                    {
                        role_Group = _mapper.Map<Admin_Role_Group>(item);
                        role_Group.userAdded = userModify;
                        role_Group.dateAdded = date;
                        _context.Admin_Role_Group.Add(role_Group);
                    }
                    else
                    {
                        role_Group = _context.Admin_Role_Group.FirstOrDefault(r => r.id == item.id);
                        role_Group.userUpdated = userModify;
                        role_Group.dateUpdated = date;
                        role_Group.role_id = item.role_id;
                        role_Group.group_id = item.group_id;
                        role_Group.is_delete = item.is_delete;
                        _context.Admin_Role_Group.Update(role_Group);
                    }
                    rolegroups.Add(role_Group);
                }
                _context.SaveChanges();
                models = _mapper.Map<List<Admin_Role_Group>>(rolegroups);
                return models;
            });
        }
        public async Task<string> RoleGroupDelete(long role_id)
        {
            return await Task.Run(() =>
            {
                string mess = "0";
                try
                {
                    var rolegroup = _context.Admin_Role_Group.FirstOrDefault(r => r.role_id == role_id);
                    rolegroup.is_delete = true;
                    _context.Admin_Role_Group.Update(rolegroup);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    mess = ex.Message;
                }
                return mess;
            });
        }
        #endregion
    }
}
