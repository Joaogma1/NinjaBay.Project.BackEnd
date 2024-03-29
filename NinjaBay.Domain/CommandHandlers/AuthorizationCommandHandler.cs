﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NinjaBay.Domain.Commands.Auth;
using NinjaBay.Domain.Contracts.Repositories;
using NinjaBay.Domain.Contracts.Services;
using NinjaBay.Domain.Entities;
using NinjaBay.Domain.Results;
using NinjaBay.Shared.Enums;
using NinjaBay.Shared.Notifications;
using NinjaBay.Shared.Persistence;
using NinjaBay.Shared.Security;
using NinjaBay.Shared.Utils;

namespace NinjaBay.Domain.CommandHandlers
{
    public class AuthorizationCommandHandler : BaseCommandHandler,
        IRequestHandler<AuthCommand, AuthResult>
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IShopperRepository _shopperRepository;
        private readonly IUserRepository _userRepository;

        public AuthorizationCommandHandler(IUnitOfWork uow, IDomainNotification notifications,
            JwtTokenConfig jwtTokenConfig, IUserRepository userRepository, IPasswordHasherService passwordHasherService,
            IShopperRepository shopperRepository) : base(uow, notifications)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _shopperRepository = shopperRepository;
        }

        public async Task<AuthResult> Handle(AuthCommand command, CancellationToken cancellationToken)
        {
            var result = new AuthResult();

            var user = await _userRepository.FindAsync(x =>
                x.Email.ToLower().Equals(command.Email.ToLower()) && x.Active);

            if (user == null)
            {
                Notifications.Notifications.Add(new Notification("Credencial invalida."));
                return result;
            }

            if (!_passwordHasherService.Check(user.Senha, command.Senha))
            {
                Notifications.Notifications.Add(new Notification("Senha invalida"));
                return result;
            }

            switch (user.Type)
            {
                case EUserType.Administrator:
                    return await HandleAdmin(user);
                case EUserType.Shopper:
                    return await HandleShopper(user);
                default:
                    return null;
            }
        }

        private async Task<AuthResult> HandleAdmin(User user)
        {
            var authResult = new AuthResult();

            var sessionUser = new SessionUser
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Nome,
                UserType = user.Type.ToString()
            };

            authResult.UserInfo = sessionUser;

            authResult.Token = _jwtTokenConfig.GenerateJwtToken(sessionUser.ClaimsPrincipal());

            return authResult;
        }

        private async Task<AuthResult> HandleShopper(User user)
        {
            var authResult = new AuthResult();

            var shopperInclude = new IncludeHelper<Shopper>()
                .Include(x => x.User)
                .Include(x => x.Addresses)
                .Includes;
            var shopper = await _shopperRepository.FindAsync(x => x.Id == user.Id, shopperInclude);

            var sessionUser = new ShopperSessionUser
            {
                Id = shopper.Id,
                Email = shopper.User.Email,
                Name = shopper.User.Nome,
                UserType = shopper.User.Type.ToString(),
                AddressInformation = shopper.Addresses!.Any() ? shopper?.Addresses?.First()?.Address : null,
                Identification = shopper.Cpf,
                AddressId = shopper.Addresses!.Any() ? shopper.Addresses?.FirstOrDefault()?.Id : null
            };

            authResult.UserInfo = sessionUser;

            authResult.Token = _jwtTokenConfig.GenerateJwtToken(sessionUser.ClaimsPrincipal());
            return authResult;
        }
    }
}