namespace ChocolateSundae.Services.Models
{
    public class UserDataProgress
    {
        public RequestStatus LoadBasicUserInfo { get; set; } = RequestStatus.NotStarted;
        public string? LoadBasicUserInfoError { get; set; } = null;
        public RequestStatus LoadFullUserInfo { get; set; } = RequestStatus.NotStarted;
        public string? LoadFullUserInfoError { get; set; } = null;
        public RequestStatus LoadUserMedia { get; set; } = RequestStatus.NotStarted;
        public string? LoadUserMediaError { get; set; } = null;
        public int LoadUserMediaCount { get; set; } = 0;
    }
}