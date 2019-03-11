namespace IIASA.Api.Global.Response
{
    public class ValueResponseModel<T>
    {
        public ValueResponseModel(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
