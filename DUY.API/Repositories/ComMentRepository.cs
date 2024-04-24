using AutoMapper;
using DUY.API.Entities;
using C.Tracking.API.IRepositories;
using C.Tracking.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using DUY.API.Model.ComMent;
using DUY.API.Model.Contract;
namespace C.Tracking.API.Repositories
{
    public class ComMentRepository : IComMentRepository
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        public ComMentRepository(ApplicationContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ComMentModel> ContractCreate(ComMentModel model)
        {
            return await Task.Run(() =>
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        ComMent contract = _mapper.Map<ComMent>(model);
                        contract.dateAdded = DateTime.Now;
                      
                        contract.is_delete = false;
                        _context.ComMents.Add(contract);
                        _context.SaveChanges();
                        string number = contract.id.ToString();
                        for (int i = number.Length; i < 5; i++)
                        {
                            number = "0" + number;
                        }
                     
                        _context.ComMents.Update(contract);
                        _context.SaveChanges();

                        model = _mapper.Map<ComMentModel>(contract);
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
        public async Task<ComMentModel> ContractUpdate(ComMentModel model)
        {
            return await Task.Run(() =>
            {
                ComMent contract = _mapper.Map<ComMent>(model);
             
                contract.dateUpdated = DateTime.Now;
                _context.ComMents.Update(contract);
                           
                _context.SaveChanges();

                return Task.FromResult(model);
            });
        }
        public async Task<bool> ContractDelete(long Contracts_id, long user_id)
        {
            return await Task.Run(async () =>
            {
                ComMent Contractss = _context.ComMents.FirstOrDefault(x => x.id == Contracts_id && !x.is_delete);
                if (Contractss != null)
                {
                    Contractss.is_delete = true;
                    Contractss.dateUpdated = DateTime.Now;
                    Contractss.userUpdated = user_id;
                }
                _context.SaveChanges();
                return true;
            });
        }
        public async Task<ComMentModel> ContractGetById(long id)
        {
            return await Task.Run(async () =>
            {
                ComMentModel model = new ComMentModel();
                ComMent contractdb = await _context.ComMents.FirstOrDefaultAsync(r => r.id == id && !r.is_delete);
                if (contractdb == null)
                    return model;
                model = _mapper.Map<ComMentModel>(contractdb);
                return model;
            });
        }
        public async Task<PaginationSet<ComMentModel>> ContractList(CommentSearch model)
        {
            return await Task.Run(async () =>
            {
                PaginationSet<ComMentModel> suppliers = new PaginationSet<ComMentModel>();

                IEnumerable<ComMentModel> listItem = from a in _context.ComMents
                                                     select new ComMentModel
                                                     {
                                                         id = a.id,
                                                         Name=a.Name,
                                                         ParentID=a.ParentID,
                                                         UserID=a.UserID,
                                                         song_Id=a.song_Id,
                                                         Content=a.Content,
                                                         Order=a.Order,
                                                         Activity=a.Activity,
                                                     };
                suppliers.lists = listItem.ToList();
                                                                                                                     
                return suppliers;
            });
        }
    }
}

