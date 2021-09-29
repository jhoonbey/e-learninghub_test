namespace app.service.Model.Response
{
    public class GenericServiceResponse<T> : ServiceResponseBase
    {
        public T Model { get; set; }
    }
}
