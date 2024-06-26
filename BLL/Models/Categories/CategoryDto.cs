﻿namespace HM.BLL.Models.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public int CategoryGroupId { get; set; }
    public string Name { get; set; } = null!;
    public int Position { get; set; }
    public string Link { get; set; } = null!;
}
