﻿namespace CarvedRock.Api.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public string Category { get; set; } = null!;
    public string ImgUrl { get; set; } = null!;
    public decimal Rating { get; set; }
    public int NumberOfRatings { get; set; }
}