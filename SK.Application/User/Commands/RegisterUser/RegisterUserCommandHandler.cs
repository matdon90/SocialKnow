﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SK.Application.Common.Exceptions;
using SK.Application.Common.Interfaces;
using SK.Application.Common.Resources.Users;
using SK.Domain.Entities;
using SK.Domain.Enums;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SK.Application.User.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IIdentityService _identityService;
        private readonly IStringLocalizer<UsersResource> _localizer;

        public RegisterUserCommandHandler(IApplicationDbContext context, IJwtGenerator jwtGenerator, IIdentityService identityService, IStringLocalizer<UsersResource> localizer)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
            _identityService = identityService;
            _localizer = localizer;
        }

        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Email = _localizer["UserRegisterEmailExistsError"] });
            }

            if (await _context.Users.AnyAsync(u => u.UserName == request.Username))
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Username = _localizer["UserRegisterUsernameExistsError"] });
            }

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var result = await _identityService.CreateUserAsync(user, request.Password);

            if (result.Result.Succeeded)
            {
                await _identityService.AddRoleToUserAsync(user, IdentityRoles.Standard.ToString());
                return new User
                {
                    Username = user.UserName,
                    Token = _jwtGenerator.CreateToken(user),
                    Image = null
                };
            }
            throw new RestException(HttpStatusCode.BadRequest, new { User = _localizer["UserSaveError"] });
        }
    }
}
