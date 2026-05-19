public class MenuCreateDto
{
    public string MenuName { get; set; } = string.Empty;

    public string? MenuCode { get; set; }

    public string? Link { get; set; }

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentMenuId { get; set; }
}