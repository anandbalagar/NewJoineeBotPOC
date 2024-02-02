using System;
using System.Collections.Generic;

namespace NewJoineeBOT.Models;

public partial class Feedback
{
    public string Rating { get; set; }

    public string Comment { get; set; }

    public int EmployeeId { get; set; }
}
