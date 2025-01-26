using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library_Managment_System.Models.Common
{
    public class BaseViewModel<TClass>
        where TClass : class
    {
        private int _itemsPerPage = 5;
        private string _orderBy;
        private Expression<Func<TClass, bool>> _filterFunc;
        private IQueryable<TClass> _itemsQueryable;

        public int CurrentPage { get; set; }
        public string Orderby
        {
            get
            {
                return _orderBy;
            }
        }

        public int NoOfPages { get; private set;  }
        public bool Is_Desc { get; set; } = false;
        public IEnumerable<TClass> Items { get; set; }
        public List<string> Errors { get; set; }

        public BaseViewModel(
            IQueryable<TClass> itemsQueryable,
            bool is_desc = false,
            int? itemsPerPage = default,
            int? currentPage = default,
            Expression<Func<TClass, bool>> func = default)
        {
            _itemsQueryable = itemsQueryable;
            _itemsPerPage = itemsPerPage ?? _itemsPerPage;
            _filterFunc = func;
            CurrentPage = currentPage ?? CurrentPage;
            Is_Desc = is_desc;
            Errors = new List<string>();

            if(_itemsPerPage < 1)
            {
                _itemsPerPage = 1;
                Errors.Add("Items per page must be at least 1");
            }
        }

        public async Task<BaseViewModel<TClass>> GetResultsAsync()
        {
            if(_filterFunc != default)
            {
                _itemsQueryable = _itemsQueryable.Where(_filterFunc);
            }

            NoOfPages = (int)Math.Ceiling((await _itemsQueryable.CountAsync())/(double)_itemsPerPage);

            if(NoOfPages < CurrentPage)
            {
               Errors.Add("Current page can't be bigger than number of pages");
               CurrentPage = 0;
            }

            _itemsQueryable = _itemsQueryable.Skip(_itemsPerPage*CurrentPage).Take(_itemsPerPage);

            Items = await _itemsQueryable.ToListAsync();

            return this;
        }

    }
}
