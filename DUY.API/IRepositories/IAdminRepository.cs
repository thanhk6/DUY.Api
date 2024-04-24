using DUY.API.Entities;

namespace DUY.API.IRepositories
{
    public interface IAdminRepository
    {
        #region admingroup
        Task<Admin_Group> GroupCreate(Admin_Group model);
        Task<Admin_Group> GroupModify(Admin_Group model);
        Task<List<Admin_Group>> GroupList(long id);
        Task<bool> GroupDelete(long id);
        #endregion

        #region adminGroupUser
        Task<List<Admin_Group_User>> GroupUserList(long group_id);
        Task<List<Admin_Group_User>> GroupUserListByGroupId(long group_id);
        Task<List<Admin_Group_User>> GroupUserCreateList(List<Admin_Group_User> models, long useradd);
        Task<List<Admin_Group_User>> GroupUserModifyList(List<Admin_Group_User> models, long userModify);
        Task<string> GroupUserDelete(long user_id);
        #endregion

        #region adminRole
        Task<Admin_Role> RoleCreate(Admin_Role model);
        Task<Admin_Role> RoleModify(Admin_Role model);
        Task<List<Admin_Role>> RoleList(long id);
        Task<bool> RoleDelete(long id);
        #endregion

        #region adminRoleGroup
        Task<List<Admin_Role_Group>> RoleGroupList(long role_id);
        Task<List<Admin_Role_Group>> RoleGroupListByGroupId(long group_id);
        Task<List<Admin_Role_Group>> RoleGroupCreateList(List<Admin_Role_Group> models, long useradd);
        Task<List<Admin_Role_Group>> RoleGroupModifyList(List<Admin_Role_Group> models, long userModify);
        Task<string> RoleGroupDelete(long role_id);
        #endregion
    }
}
