using System.Collections.Generic;

namespace TdDb
{
    public class PagedList<T>
    {
        public PagedList()
        {
            this.HasNext = false;
            this.HasPrevious = false;
            this.Rows = new T[0];
            this.TotalRows = 0;
            this.TotalPages = 0;
        }

        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }

        public int TotalPages { get; set; }

        public int TotalRows { get; set; }

        public IEnumerable<T> Rows { get; set; }
    }
}
