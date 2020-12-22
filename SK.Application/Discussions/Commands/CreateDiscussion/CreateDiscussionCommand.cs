﻿using MediatR;
using System;

namespace SK.Application.Discussions.Commands.CreateDiscussion
{
    public class CreateDiscussionCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PostBody { get; set; }
        public CreateDiscussionCommand() { }
        public CreateDiscussionCommand(DiscussionCreateDto request)
        {
            Id = request.Id;
            Title = request.Title;
            Description = request.Description;
            PostBody = request.PostBody;
        }
    }
}
