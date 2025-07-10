using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyPriceTracker.Application.DTOs.ServiceResult {
    /// <summary>
    /// Frontend'e giden isteklerin tutarlı olmasını sağlar. Non-generic ServiceResult
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public class ServiceResult {
        public bool IsSuccess { get; set; }       // API'yı tüketen servisler ilk buna bakar
        public string? Message { get; set; }      // genel mesaj, zorunlu değildir, null dönebilir
        public List<string>? Errors { get; set; } // birden fazla hata mesajı, nullable

        /// <summary>
        /// Başarılı durumlar için
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns
        public static ServiceResult Success(string? message = null) {
            return new ServiceResult { IsSuccess = true, Message = message };
        }

        /// <summary>
        /// Başarısız durumlar için
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns
        public static ServiceResult Failure(string? message = null, List<string>? errors = null) {
            return new ServiceResult { IsSuccess = false, Message = message, Errors = errors };
        }
        public static ServiceResult Failure(string error, string? message = null) {
            return new ServiceResult { IsSuccess = false, Message = message, Errors = new List<string> { error } };
        }
    }

    /// <summary>
    /// Generic ServiceResult
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns
    public class ServiceResult<T> : ServiceResult {
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data, string? message = null) {
            return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static ServiceResult<T> Failure(string? message = null, List<string>? errors = null) {
            return new ServiceResult<T> { IsSuccess = false, Message = message, Errors = errors };
        }
        public static ServiceResult<T> Failure(string error, string? message = null) {
            return new ServiceResult<T> { IsSuccess = false, Message = message, Errors = new List<string> { error } };
        }

    }
}
