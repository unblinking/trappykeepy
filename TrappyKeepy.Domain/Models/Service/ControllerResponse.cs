namespace TrappyKeepy.Domain.Models
{
    public class ControllerResponse
    {
        /// <summary>
        /// Success, fail, or error.
        /// </summary>
        /// <value></value>
        public string? Status { get; set; }

        /// <summary>
        /// A human readable message for the client's benefit.
        /// </summary>
        /// <value></value>
        public string? Message { get; set; }

        /// <summary>
        /// An internal error code or trace ID that could be helpful if the
        /// client seeks additional assistance to resolve an issue. Could be
        /// something like a searchable code in an online knowledge base.
        /// </summary>
        /// <value></value>
        public string? Code { get; set; }

        /// <summary>
        /// Data that the client requested.
        /// </summary>
        /// <value></value>
        public object? Data { get; set; }

        /// <summary>
        /// Internal server error. Provide no details to the client.
        /// </summary>
        public void Error()
        {
            this.Status = "error";
            this.Message = "An error occurred while processing your request.";
        }

        /// <summary>
        /// Client error. Provide feedback to client if available.
        /// </summary>
        /// <param name="message"></param>
        public void Fail(string? message)
        {
            this.Status = "fail";
            this.Message = message;
        }

        /// <summary>
        /// Successful action. Provide requested data to client if applicable.
        /// </summary>
        /// <param name="data"></param>
        public void Success(object? data)
        {
            this.Status = "success";
            this.Data = data;
        }
    }
}