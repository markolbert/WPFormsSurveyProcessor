﻿namespace WPFormsSurvey;

[JsonFieldName("text")]
public class TextField : FieldBase
{
    public string Label { get; set; } = string.Empty;
}