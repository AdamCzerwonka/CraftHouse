﻿using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages;

[RequireAuth]
public class OrderModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly IOrderRepository _orderRepository;

    public OrderModel(IAuthService authService, ICartService cartService,
        IOrderRepository orderRepository)
    {
        _authService = authService;
        _cartService = cartService;
        _orderRepository = orderRepository;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var cart = _cartService.GetCartEntries();
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);


        await _orderRepository.CreateOrderAsync(cart, user!, cancellationToken);
        
        _cartService.ClearCart();

        return Redirect("/Index");
    }
}