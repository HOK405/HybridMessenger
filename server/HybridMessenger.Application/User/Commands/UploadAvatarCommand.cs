using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridMessenger.Application.User.Commands
{
    public class UploadAvatarCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public IFormFile File { get; set; }
    }
}