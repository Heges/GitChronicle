namespace Utils.Core.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; } = false;
        public Dictionary<string, object> Data { get; set; }
        public List<string> Messages { get; set; }
        public List<string> Errors { get; set; }

        public Result Ok(Dictionary<string, object> data, params string[] messages) 
        {
            return new Result()
            {
                IsSuccess = true,
                Data = data,
                Messages = new List<string>(messages),
                Errors = new List<string>()
            };
        }

        public Result Fail(params string[] errors)
        {
            return new Result()
            {
                IsSuccess = false,
                Data = new Dictionary<string, object>(),
                Messages = new List<string>(),
                Errors = new List<string>(errors)
            };
        }
    }
}
