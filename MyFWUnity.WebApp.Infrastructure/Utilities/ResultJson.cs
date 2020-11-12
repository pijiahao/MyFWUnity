using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.WebApp.Infrastructure.Model.ResultData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.WebApp.Infrastructure.Utilities
{
    public class ResultJson
    {
        public static HttpResponseMessage BuildExceptionJsonResponse(HttpStatusCode code, Exception ex)
        {
            string error = ExceptionHelper.GetMessage(ex);
            LogModule.Error(error, ex);
            return BuildJsonResponse(code, null, MessageType.Error, ex.Message);
        }

        public static HttpResponseMessage BuildNullJsonResponse(HttpStatusCode code, string Message)
        {
            LogModule.Error(Message);
            return BuildJsonResponse(code, null, MessageType.None, Message);
        }

        public static HttpResponseMessage BuildEmptySuccessJsonResponse()
        {
            return BuildJsonResponse(HttpStatusCode.OK, null, MessageType.None, string.Empty);
        }

        public static HttpResponseMessage BuildJsonResponse(Object resultData, MessageType type = MessageType.None, string message = "")
        {
            return BuildJsonResponse(HttpStatusCode.OK, resultData, type, message);
        }

        public static HttpResponseMessage BuildJsonResponse(HttpStatusCode code, Object resultData, MessageType type, string message)
        {
            HttpResponseMessage result = null;
            try
            {
                bool hasMessage = !string.IsNullOrEmpty(message);
                MessageModel msgModel = hasMessage ? new MessageModel(type, message) : null;
                ResultDataBag lobjResult = new ResultDataBag(hasMessage, msgModel, resultData);

                String str = lobjResult.ToJson();
                result = new HttpResponseMessage { StatusCode = code, Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            catch (Exception ex)
            {
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(ex.Message, Encoding.GetEncoding("UTF-8"), "application/json") };
                LogModule.Error(ex.Message, ex);
            }
            return result;
        }

        public static HttpResponseMessage SuccesstoJson(Object obj, string Message)
        {
            HttpResponseMessage result = null;
            try
            {
                String str = "";
                if (obj != null)
                {
                    if (obj is String || obj is Char)
                    {
                        str = obj.ToString();
                    }
                    else
                    {
                        str = obj.ToJson();
                    }
                    if (obj is bool)
                    {
                        str = obj.ToString().ToLower();
                    }
                }
                if (!string.IsNullOrEmpty(Message))
                {
                    LogModule.Debug(Message);
                }
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            catch (Exception ex)
            {
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
                LogModule.Error(ex.Message);
            }
            return result;
        }

        public static HttpResponseMessage FailtoJson(string Message, HttpStatusCode HttpStatusCode)
        {
            HttpResponseMessage result = null;
            try
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    LogModule.Debug(Message);
                }
                bool hasMessage = !string.IsNullOrEmpty(Message);
                MessageModel msgModel = hasMessage ? new MessageModel(MessageType.Error, Message) : null;
                ResultDataBag lobjResult = new ResultDataBag(hasMessage, msgModel, null);
                String str = lobjResult.ToJson();
                result = new HttpResponseMessage { StatusCode = HttpStatusCode, Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            catch (Exception ex)
            {
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
                LogModule.Error(ex.Message);
            }
            return result;
        }

        public static HttpResponseMessage ErrortoJson(Object obj, string ErrorMessage)
        {
            HttpResponseMessage result = null;
            try
            {
                String str = "";
                if (obj != null)
                {
                    if (obj is String || obj is Char)
                    {
                        str = obj.ToString();
                    }
                    else
                    {
                        str = obj.ToJson();
                    }
                }
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    LogModule.Error(ErrorMessage);
                }
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            catch (Exception ex)
            {
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
                LogModule.Error(ex.Message);
            }
            return result;
        }

        public static HttpResponseMessage ErrorToJson(Object obj, HttpStatusCode code, Exception exception)
        {
            HttpResponseMessage result = null;
            try
            {
                if (exception != null)
                {
                    LogModule.Error(ExceptionHelper.GetMessage(exception), exception);
                }

                String str = "";
                if (obj != null)
                {
                    if (obj is String || obj is Char)
                    {
                        str = obj.ToString();
                    }
                    else
                    {
                        str = obj.ToJson();
                    }
                }

                result = new HttpResponseMessage { StatusCode = code, Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            catch (Exception ex)
            {
                string message = ExceptionHelper.GetMessage(ex);
                result = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(message, Encoding.GetEncoding("UTF-8"), "application/json") };
                LogModule.Error(ExceptionHelper.GetMessage(ex), ex);
            }
            return result;
        }
    }
}
