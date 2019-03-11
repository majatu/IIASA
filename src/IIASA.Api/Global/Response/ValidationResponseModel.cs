using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace IIASA.Api.Global.Response
{
    public class ValidationFailedResultModel : ObjectResult
    {
        public ValidationFailedResultModel(ModelStateDictionary modelState)
            : base(new ValidationResultModel(modelState))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    public class ValidationResultModel : ValidationResponseModel
    {
        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationResponseItemModel(key, x.ErrorMessage)))
                .ToList();
        }
    }

    public class ValidationResponseModel
    {
        public string Message { get; set; }

        public List<ValidationResponseItemModel> Errors { get; set; }

        public ValidationResponseModel()
        {
            Errors = new List<ValidationResponseItemModel>();
        }
    }

    public class ValidationResponseItemModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        public string Message { get; set; }

        public ValidationResponseItemModel(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}