using MediatR;
using FlameRestaurant.Application.AppCode.Infrastructure;
using FlameRestaurant.Domain.Models.DbContexts;
using FlameRestaurant.Domain.Models.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlameRestaurant.Domain.Business.BlogPostModule
{
    public class BlogPostAdminGetAllQuery : PaginateModel, IRequest<PagedViewModel<BlogPost>>
    {
        public class BlogPostAdminGetAllQueryHandLer : IRequestHandler<BlogPostAdminGetAllQuery, PagedViewModel<BlogPost>>
        {
            private readonly FlameRestaurantDbContext db;

            public BlogPostAdminGetAllQueryHandLer(FlameRestaurantDbContext db)
            {
                this.db = db;
            }
            public async Task<PagedViewModel<BlogPost>> Handle(BlogPostAdminGetAllQuery request, CancellationToken cancellationToken)
            {
                if (request.PageSize < 6)
                {
                    request.PageSize = 6;
                }
                var query = db.BlogPosts
                    .Where(bp => bp.DeletedDate == null)
                    .AsQueryable();
                if(query == null)
                {
                    return null;
                }
                var pagedModel = new PagedViewModel<BlogPost>(query, request);
                return pagedModel;
            }
        }
    }
}
