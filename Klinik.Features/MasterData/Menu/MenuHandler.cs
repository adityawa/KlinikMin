using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.MasterData;
using LinqKit;
using System.Collections.Generic;
using System.Linq;

namespace Klinik.Features
{
    public class MenuHandler : BaseFeatures
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MenuHandler()
        {
        }

        /// <summary>
        /// Contructor with parameter
        /// </summary>
        /// <param name="unitOfWork"></param>
        public MenuHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get visible menu based on level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="parentmenuid"></param>
        /// <returns></returns>
        public IList<MenuModel> GetVisibleMenu(int level = 0, int parentmenuid = 0)
        {
            IList<MenuModel> menus = new List<MenuModel>();
            var qryPredicate = PredicateBuilder.New<Menu>(true);
            qryPredicate = qryPredicate.And(x => x.IsMenu == true);
            if (level > 0)
                qryPredicate = qryPredicate.And(x => x.Level == level);
            if (parentmenuid > 0)
                qryPredicate = qryPredicate.And(x => x.ParentMenuId == parentmenuid);
            var qry = _unitOfWork.MenuRepository.Get(qryPredicate);

            foreach (var item in qry)
            {
                var _mdl = Mapper.Map<Menu, MenuModel>(item);
                menus.Add(_mdl);
            }

            return menus;
        }

        /// <summary>
        /// Get menu based on privilege
        /// </summary>
        /// <param name="privileges"></param>
        /// <returns></returns>
        public IList<MenuModel> GetMenuBasedOnPrivilege(List<long> privileges)
        {
            var qry_menuid = _unitOfWork.PrivilegeRepository.Get(x => privileges.Contains(x.ID)).Select(x => x.MenuID);
            var qry2menu = _unitOfWork.MenuRepository.Get(x => qry_menuid.ToList().Contains(x.ID), orderBy: q => q.OrderBy(x => x.Level).ThenBy(x => x.SortIndex));
            IList<MenuModel> _authmenu = new List<MenuModel>();
            foreach (var item in qry2menu)
            {
                var _menu = Mapper.Map<Menu, MenuModel>(item);
                _authmenu.Add(_menu);

            }
            return _authmenu;
        }
    }
}