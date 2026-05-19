public class MenuUpdateDto
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = string.Empty;

    public string? MenuCode { get; set; }

    public string? Link { get; set; }

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentMenuId { get; set; }
    public bool? IsActive { get; set; }



}