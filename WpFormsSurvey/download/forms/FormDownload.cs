using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace WpFormsSurvey;

public class FormDownload
{
    public DownloadHeader? Header { get; set; } 
    public DownloadDatabase? Database { get; set; }
    public DownloadFormTable? Table { get; set; }

    public bool IsValid => Header != null && Database != null && Table is { Data: {} };
}