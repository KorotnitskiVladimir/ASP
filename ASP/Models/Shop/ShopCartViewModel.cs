﻿using ASP.Data.Entities;

namespace ASP.Models.Shop;

public class ShopCartViewModel
{
    public Cart? Cart { get; set; }

    public List<Product>? Products { get; set; }
}