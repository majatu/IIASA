using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIASA.Api.Global.Response
{
    public interface IErrorResponseModel
    {
        int InternalCode { get; }

        string InternalMessage { get; }
    }

    public class ErrorResponseModel : IErrorResponseModel
    {
        public int InternalCode { get; set; }

        public string InternalMessage { get; set; }
    }


    public class ErrorResponsePredefinedModel : IErrorResponseModel
    {
        private readonly string _value;

        public ErrorResponsePredefinedModel(string value)
        {
            _value = value;
        }


        public int InternalCode => 500;

        public string InternalMessage => _value;
    }

    public class ErrorResponseEnumModel<T> : IErrorResponseModel
        where T : struct, IConvertible
    {
        private readonly T _value;

        public ErrorResponseEnumModel(T value)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T not Enum.");

            _value = value;
        }

        public int InternalCode => (int)(object)_value;

        public string InternalMessage => ((object)_value).ToString();
    }
}