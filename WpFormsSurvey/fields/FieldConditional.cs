﻿namespace J4JSoftware.WpFormsSurvey;

public class FieldConditional
{
    public int LinkedFieldId { get; set; }
    public string Operator { get; set; } = string.Empty;
    public int LinkedChoiceId { get; set; }
}
