using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common
{
    public class ExceptionHelper
    {
        /// <summary>
        /// Get the actual error message
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessage(System.Exception ex)
        {
            System.Exception interEx = ex;
            if (ex.GetBaseException() != null)
                interEx = ex.GetBaseException();
            return interEx.Message;
        }
    }
}
