﻿namespace WpFormsSurvey;

public record UserFieldResponse<T>(
    int UserId,
    int FormId,
    string IpAddress,
    DateTime Submitted,
    int FieldId,
    T Response
) : IUserFieldResponse<T>
    where T : notnull
{
    public object GetResponse() => Response;
}