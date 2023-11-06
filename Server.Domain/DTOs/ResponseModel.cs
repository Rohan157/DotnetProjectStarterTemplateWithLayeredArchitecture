namespace Server.Domain.DTOs
{
    public class ResponseModel
    {
        public ResponseModel(string message, bool data)
        {
            Message = message;
            Data = data;
        }
        public string Message { get; set; }
        public bool Data { get; set; }
    }
}
