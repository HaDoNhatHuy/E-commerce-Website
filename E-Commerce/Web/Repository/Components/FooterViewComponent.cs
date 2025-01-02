﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Repository.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public FooterViewComponent(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Contacts.FirstOrDefaultAsync());//lấy database ra trả ra view
    }
}
