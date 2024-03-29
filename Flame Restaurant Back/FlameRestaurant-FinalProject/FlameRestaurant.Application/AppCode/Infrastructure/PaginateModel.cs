﻿namespace FlameRestaurant.Application.AppCode.Infrastructure
{
    public abstract class PaginateModel
    {
        int pageIndex;
        int pageSize;
        public int PageIndex
        {
            get
            {
                return this.pageIndex < 1 ? 1 : this.pageIndex;
            }
            set
            {
                if (value >= 1)
                {
                    this.pageIndex = value;
                }
            }
        }

        public virtual int PageSize
        {
            get
            {
                return this.pageSize < 5 ? 5 : this.pageSize;
            }
            set
            {
                if (value >= 5)
                {
                    this.pageSize = value;
                }
            }
        }
    }
}
