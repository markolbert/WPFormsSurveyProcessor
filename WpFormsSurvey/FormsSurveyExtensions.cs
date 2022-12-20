using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpFormsSurvey;

public static class FormsSurveyExtensions
{
    public static List<FormInfo> ToFormInfo(this Forms forms)
    {
        return forms.Data?.ToFormInfo() ?? new List<FormInfo>();
    }

    public static List<FormInfo> ToFormInfo(this List<Form> forms)
    {
        return forms.Select(x => new FormInfo(x.Id, x.PostTitle))
                    .Distinct(FormInfo.DefaultComparer)
                    .OrderBy(x => x.Id)
                    .ToList();
    }
}
