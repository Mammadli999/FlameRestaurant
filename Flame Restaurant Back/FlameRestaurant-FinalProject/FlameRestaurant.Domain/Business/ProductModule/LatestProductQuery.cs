using FlameRestaurant.Domain.Models.DbContexts;
using FlameRestaurant.Domain.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlameRestaurant.Domain.Business.ProductModule
{
    public class LatestProductQuery : IRequest<List<Product>>
    {

        public int Size { get; set; }

        public class LatestProductQueryHandler : IRequestHandler<LatestProductQuery, List<Product>>
        {
            private readonly FlameRestaurantDbContext db;

            public LatestProductQueryHandler(FlameRestaurantDbContext db)
            {
                this.db = db;
            }
            public async Task<List<Product>> Handle(LatestProductQuery request, CancellationToken cancellationToken)
            {
                var posts = await db.Products
                     .Where(bp => bp.DeletedDate == null)
                     .OrderByDescending(bp => bp.CreatedDate)
                     .Take(request.Size < 4 ? 4 : request.Size)
                     .ToListAsync(cancellationToken);

                return posts;
            }
        }

    }
}
