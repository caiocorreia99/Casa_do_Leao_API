using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Api
{
    public class PaggedList<T> where T : class
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageRange { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }

        public PaggedList()
        {

        }

        public PaggedList(int page, int size, int pageRange, int totalCount, List<T> items)
        {
            this.Page = page;
            this.PageSize = size;
            this.PageRange = pageRange;
            this.TotalCount = totalCount;
            this.Items = items;
        }

    }
}
