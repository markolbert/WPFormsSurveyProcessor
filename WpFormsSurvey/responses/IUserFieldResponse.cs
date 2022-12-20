namespace WpFormsSurvey;

public interface IUserFieldResponse
{
    int UserId { get; }
    int FormId { get; }
    string IpAddress { get; }
    DateTime Submitted { get; }
    int FieldId { get; }

    object GetResponse();
}

public interface IUserFieldResponse<out T> : IUserFieldResponse
    where T : notnull
{
    T Response { get; }
}

