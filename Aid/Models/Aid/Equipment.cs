using System;
using System.Collections.Generic;

namespace Aid.Models.Aid;

public partial class Equipment
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public int? Strength { get; set; }
    public int? StrengthLeft { get; set; }

    public DateTime? Date { get; set; }
}
