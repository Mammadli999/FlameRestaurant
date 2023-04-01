using FlameRestaurant.Domain.Business.ProductModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlameRestaurant.WebUI.AppCode.ViewComponents.LatestProducts
{
    public class LatestProductsViewComponent : ViewComponent
    {
        private readonly IMediator mediator;

        public LatestProductsViewComponent(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var query = new LatestProductQuery();
            var products = await mediator.Send(query);

            return View(products);
        }
    }
}
