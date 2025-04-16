namespace ProjectManagement.Domain.Models
{ 
    public class BaseResponse<T> 
    {
        public bool IsSuccess { get; set; } = false;
        public List<string> Message { get; set; } = [];
        public List<T> ListResponse { get; set; } = [];
        public int? ErrorCode { get; set; } = null;
    }
}
