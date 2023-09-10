using System;
namespace FDex.Application.Responses.Common
{
    public abstract class BaseCommandResponse
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}

