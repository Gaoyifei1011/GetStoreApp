using System;

namespace GetStoreApp.Extensions.DataType.Exceptions
{
    /// <summary>
    /// 进程不存在异常
    /// </summary>
    public class ProcessNotExistException : ApplicationException
    {
        private string error;
        private Exception innerException;

        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public ProcessNotExistException()
        {
        }

        /// <summary>
        /// 带一个字符串参数的构造函数
        /// </summary>
        public ProcessNotExistException(string msg)
        {
            error = msg;
        }

        /// <summary>
        /// 带有一个字符串参数和一个内部异常信息参数的构造函数
        /// </summary>
        public ProcessNotExistException(string msg, Exception exception)
        {
            innerException = exception;
            error = msg;
        }

        public string GetError()
        {
            if (error is not null)
            {
                return error;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        public Exception GetInnerException()
        {
            if (innerException is not null)
            {
                return innerException;
            }
            else
            {
                return default;
            }
        }
    }
}
