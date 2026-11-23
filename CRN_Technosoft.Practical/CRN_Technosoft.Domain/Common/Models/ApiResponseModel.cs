namespace CRN_Technosoft.Domain.Common.Models
{
    public class ApiResponseModel
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public object payload { get; set; } = new object();
        public Guid requestId { get; set; }
        public string[] errors { get; set; } = Array.Empty<string>();

        public ApiResponseModel()
        {
            requestId = Guid.NewGuid();
            statusCode = 400;
            payload = "";
            message = string.Empty ?? "bad request";
            errors = Array.Empty<string>();
        }
        public ApiResponseModel(string message, int statusCode, object payload)
        {
            requestId = Guid.NewGuid();
            this.statusCode = statusCode;
            this.payload = payload;
            this.message = message ?? "Success";
        }

        public ApiResponseModel(string[] errors)
        {
            requestId = Guid.NewGuid();
            statusCode = 500;
            payload = "";
            message = "bad request";
            this.errors = errors ?? Array.Empty<string>();
        }

    }
}
