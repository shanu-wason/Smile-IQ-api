using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile_IQ.Application.Exceptions
{
    public class ImageValidationException : Exception
    {
        public ImageValidationException()
            : base("The uploaded image does not appear to contain dental/teeth imagery. Please upload a clear photo of teeth or a smile.")
        {
        }
        public ImageValidationException(string message)
            : base(message)
        {
        }
    }
}
