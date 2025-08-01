using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolInformationSystem.Application.DTOs
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string? Message { get; set; }

        public static ServiceResponse<T> Success(T data)
        {
            return new ServiceResponse<T> { Data = data, IsSuccess = true };
        }

        public static ServiceResponse<T> Fail(string errorMessage)
        {
            return new ServiceResponse<T> { IsSuccess = false, Message = errorMessage };
        }
    }
}
