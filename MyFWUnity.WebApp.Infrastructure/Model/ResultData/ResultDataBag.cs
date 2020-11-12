using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.WebApp.Infrastructure.Model.ResultData
{
    public enum MessageType
    {
        /// <summary>
        /// No need to show message, default
        /// </summary>
        None,
        Information,
        Error,
        Warning,
        Question
    }

    /// <summary>
    /// Use MessageCode or Message, either one, not both. 
    /// Once we can get message by code in JS, we could remove message
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Indicates result type
        /// </summary>
        public MessageType Type { get; private set; }

        /// <summary>
        /// To support globalization, JS could get resource by code from different resource file
        /// </summary>
        //public string MessageKey { get; private set; }

        /// <summary>
        /// To support globalization, low level returns resource depends on system OS language 
        /// </summary>
        public string Message { get; private set; }

        //public MessageModel(string messageKey, string message)
        //{
        //    MessageKey = messageKey;
        //    if (!string.IsNullOrEmpty(messageKey))
        //    {
        //        Message = GetMessageByKey(messageKey);
        //    }
        //    else
        //    {
        //        Message = message;
        //    }
        //}

        //private string GetMessageByKey(string messageKey)
        //{
        //    throw new NotImplementedException();
        //}

        public MessageModel(MessageType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    /// <summary>
    /// Used for ApiController which return result
    /// </summary>
    public class ResultDataBag
    {
        /// <summary>
        /// Indicates whether show message form
        /// </summary>
        public bool HasMessage { get; private set; }

        /// <summary>
        /// Get the message for displaying
        /// </summary>
        public MessageModel MessageModel { get; private set; }

        /// <summary>
        /// Get the actual result data, depends on business requirement, 
        /// it may be a simple object, or a list of object, or a complex object with other properties
        /// it also may be null if fail to get result
        /// </summary>
        public object ResultData { get; set; }

        public ResultDataBag(bool hasMessage, MessageModel messageModel, object resultData)
        {
            HasMessage = hasMessage;
            MessageModel = messageModel;
            ResultData = resultData;
        }
    }
}
