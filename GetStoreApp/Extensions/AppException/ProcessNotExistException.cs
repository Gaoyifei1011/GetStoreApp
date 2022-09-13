using System;

namespace GetStoreApp.Extensions.AppException
{
    /// <summary>
    /// 进程不存在异常
    /// </summary>
    public class ProcessNotExistException : ApplicationException
    {
        private string error;
        private Exception innerException;

        //无参数构造函数
        public ProcessNotExistException()
        {
        }

        //带一个字符串参数的构造函数
        public ProcessNotExistException(string msg)
        {
            error = msg;
        }

        //带有一个字符串参数和一个内部异常信息参数的构造函数
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

        public Exception GetInnerException()
        {
            if (innerException is not null)
            {
                return innerException;
            }
            else
            {
                return default(Exception);
            }
        }
    }
}
