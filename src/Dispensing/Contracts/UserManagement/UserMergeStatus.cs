namespace CareFusion.Dispensing.Contracts
{
    public enum UserMergeStatusCode
    {
        UserMergeSuccessful,
        UserMergeError
    }
    public class UserMergeStatus
    {
        public UserMergeStatusCode Code { get; set; }

        public string Description { get; set; }
    }
}
