﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NinjaBay.Domain.Contracts.Repositories;
using NinjaBay.Domain.Entities;
using NinjaBay.Domain.Projections;
using NinjaBay.Domain.Queries.Order;
using NinjaBay.Domain.ViewModels;
using NinjaBay.Shared.Notifications;
using NinjaBay.Shared.Paging;
using NinjaBay.Shared.Utils;

namespace NinjaBay.Domain.QueryHandler
{
    public class OrderQueryHandler : BaseQueryHandler,
        IRequestHandler<PagedOrderListQuery, PagedList<OrderVm>>,
        IRequestHandler<PagedAllOrdersListQuery, PagedList<OrderVm>>,
        IRequestHandler<GetOrderByIdQuery, OrderVm>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderQueryHandler(IDomainNotification notifications, IOrderRepository orderRepository) : base(
            notifications)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderVm> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var includes = new IncludeHelper<Order>()
                .Include(x => x.ShippingAddress)
                .Include(x => x.Products)
                .Include(x => x.Products.Select(y => y.Product))
                .Include(x => x.Products.Select(y => y.Product).Select(i => i.Images))
                .Includes;
            return (await _orderRepository.FindAsync(x =>
                x.Id == query.Id, includes)).ToVm();
        }

        public async Task<PagedList<OrderVm>> Handle(PagedAllOrdersListQuery query, CancellationToken cancellationToken)
        {
            var where = PredicateBuilder.True<Order>();

            var count = await _orderRepository.CountAsync(where);

            var includes = new IncludeHelper<Order>()
                .Include(x => x.ShippingAddress)
                .Include(x => x.Products)
                .Include(x => x.Products.Select(y => y.Product))
                .Include(x => x.Products.Select(y => y.Product).Select(i => i.Images))
                .Includes;

            var orders = _orderRepository.List(where, query.Filter, includes).ToVm();

            return new PagedList<OrderVm>(orders, count, query.Filter.PageSize);
        }

        public async Task<PagedList<OrderVm>> Handle(PagedOrderListQuery query, CancellationToken cancellationToken)
        {
            var where = PredicateBuilder.True<Order>().And(x => x.ShopperId == query.SessionUser.Id);

            var count = await _orderRepository.CountAsync(where);

            var includes = new IncludeHelper<Order>()
                .Include(x => x.ShippingAddress)
                .Include(x => x.Products)
                .Include(x => x.Products.Select(y => y.Product))
                .Include(x => x.Products.Select(y => y.Product).Select(i => i.Images))
                .Includes;

            var orders = _orderRepository.List(where, query.Filter, includes).ToVm();

            return new PagedList<OrderVm>(orders, count, query.Filter.PageSize);
        }
    }
}