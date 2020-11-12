using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace MyFWUnity.Common.MSMQ
{
    public class MSMQHelper
    {
        private readonly string _path;
        private MessageQueue _msmq;
        public MSMQHelper(string name)
        {
            _path = @".\private$\"+ name;  //这是本机实例的方式
            if (!MessageQueue.Exists(_path))
            {
                MessageQueue.Create(_path);
            }
            _msmq = new MessageQueue(_path);
        }

        /// <summary>
        /// 发送消息队列
        /// </summary>
        /// <param name="msmqIndex">消息队列实体</param>
        /// <returns></returns>
        public void Send(object msmqIndex)
        {
            _msmq.Send(new Message(msmqIndex, new BinaryMessageFormatter()));
        }

        /// <summary>
        /// 接收消息队列,删除队列
        /// </summary>
        /// <returns></returns>
        public object ReceiveAndRemove()
        {
            object msmqIndex = null;
            _msmq.Formatter = new BinaryMessageFormatter();
            Message msg = null;
            try
            {
                msg = _msmq.Receive(new TimeSpan(0, 0, 1));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //做日志记录和发送邮件报警
            }
            if (msg != null)
            {
                msmqIndex = msg.Body;
            }
            return msmqIndex;
        }


        /// <summary>
        /// 释放消息队列实例
        /// </summary>
        public void Dispose()
        {
            if (_msmq != null)
            {
                _msmq.Close();
                _msmq.Dispose();
                _msmq = null;
            }
        }
    }
}
