﻿using MediatR;
using SK.Application.Common.Exceptions;
using SK.Application.Common.Interfaces;
using SK.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Localization;
using SK.Application.Common.Resources.Discussions;

namespace SK.Application.Discussions.Commands.CloseDiscussion
{
    public class CloseDiscussionCommandHandler : IRequestHandler<CloseDiscussionCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<DiscussionsResource> _localizer;

        public CloseDiscussionCommandHandler(IApplicationDbContext context, IStringLocalizer<DiscussionsResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(CloseDiscussionCommand request, CancellationToken cancellationToken)
        {
            var discussionToClose = await _context.Discussions.FindAsync(request.Id) ?? throw new NotFoundException(nameof(Discussion), request.Id);

            if (discussionToClose.IsClosed)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Discussion = _localizer["DiscussionCloseError"] });
            }

            discussionToClose.IsClosed = true;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
            {
                return Unit.Value;
            }
            throw new RestException(HttpStatusCode.BadRequest, new { Discussion = _localizer["DiscussionSaveError"] });
        }
    }
}
