using Microsoft.AspNetCore.Mvc;
using work_01_Session.Models;

namespace work_01_Session.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly ECommerceDbContext dbContext = null;
        public ShoppingController(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            ViewBag.msg = TempData["msg"];
            return View(dbContext.Products.ToList());
        }
        //public IActionResult Index()
        //{
        //    ViewBag.msg = TempData["msg"];
        //    var dbContext = new ECommerceDbContext();
        //    ViewBag.DbContext = dbContext;
        //    return View(dbContext.Products.ToList());
        //}

        public IActionResult AddToCart(int pid, double qty)
        {
            bool itemFound = false;
            string msg = "";

            if (pid > 0 && qty > 0)
            {
                var prod = dbContext.Products.FirstOrDefault(p => p.Id == pid);
                if (prod != null)
                {
                    if (qty > prod.Quantity)
                    {
                        msg = "You can't buy more than our current stock.";
                    }
                    else
                    {
                        List<Product> items = new List<Product>();
                        var xitems = HttpContext.Session.GetObject<List<Product>>("cart");
                        if (xitems != null)
                        {
                            foreach (var item in xitems)
                            {
                                if (pid == item.Id)
                                {
                                    item.Quantity += qty;
                                    itemFound = true;
                                    msg = "Item already added, new qty is added!!!";
                                }
                                items.Add(item);
                            }
                            if (!itemFound)
                            {
                                prod.Quantity = qty;
                                items.Add(prod);
                                msg = "New Item is added with existing items!!!";
                            }
                            HttpContext.Session.SetObject<List<Product>>("cart", items);
                        }
                        else
                        {
                            prod.Quantity = qty;
                            items.Add(prod);
                            HttpContext.Session.SetObject<List<Product>>("cart", items);
                            msg = "Item is added to empty cart!!";
                        }
                    }
                }
                else
                {
                    msg = "Item not found!!!";
                }
            }
            else
            {
                msg = "Item Id or qty could not be zero!!!";
            }

            TempData["msg"] = msg;
            return RedirectToAction("Index");
        }
        //public IActionResult ShowCart()
        //{
        //    List<Product> items = HttpContext.Session.GetObject<List<Product>>("cart");
        //    if (items != null && items.Count != 0)
        //    {
        //        return View(items.ToList());
        //    }
        //    else
        //    {
        //        items = new List<Product>();
        //        return View();
        //    }
        //}
        public IActionResult ShowCart()
        {
            List<Product> items = HttpContext.Session.GetObject<List<Product>>("cart");
            List<Product> updatedItems = new List<Product>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var prod = dbContext.Products.FirstOrDefault(p => p.Id == item.Id);
                    if (prod != null)
                    {
                        updatedItems.Add(item);
                    }
                }
            }

            if (updatedItems.Count != 0)
            {
                return View(updatedItems);
            }
            else
            {
                return View();
            }
        }

        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<Product> productList = HttpContext.Session.GetObject<List<Product>>("cart");
            var removeItem = productList.FirstOrDefault(x => x.Id == id);
            productList.Remove(removeItem);
            HttpContext.Session.SetObject<List<Product>>("cart", productList);
            return RedirectToAction(nameof(ShowCart));
        }
        //public IActionResult RemoveFromCart(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    List<Product> productList = HttpContext.Session.GetObject<List<Product>>("cart");
        //    var removeItem = productList.FirstOrDefault(x => x.Id == id);
        //    if (removeItem != null)
        //    {
        //        productList.Remove(removeItem);
        //        HttpContext.Session.SetObject<List<Product>>("cart", productList);
        //    }

        //    return RedirectToAction(nameof(ShowCart));
        //}

        public IActionResult CheckOut()
        {
            var us = HttpContext.Session.GetString("un");
            var id = HttpContext.Session.GetString("id");

            if(us != null)
            {
                return RedirectToAction(nameof(ConfirmOrder));
            }
            else
            {
                return View(nameof(Login));
            }
        }
        public IActionResult ConfirmOrder()
        {
            return View();
        }
        public IActionResult Login(AppUser appUser)
        {
            var userName = dbContext.AppUsers.FirstOrDefault(x => x.UserName == appUser.UserName);
            var password = dbContext.AppUsers.FirstOrDefault(x => x.Password == appUser.Password);
            if (userName != null && password != null)
            {
                HttpContext.Session.SetString("un", appUser.UserName);
                HttpContext.Session.SetString("id", appUser.Password);
                return RedirectToAction(nameof(ConfirmOrder));

            }
            else
            {
                TempData["wrongInfo"] = "Wrong Information!!";
                return View(appUser);
            }
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("UserName, Password")] AppUser appUser)
        {
            string msg = "";
            if (ModelState.IsValid)
            {
                appUser.Role = 1;
                appUser.IsActive = true;
                var checkUserName = dbContext.AppUsers.FirstOrDefault(x => x.UserName.ToLower() == appUser.UserName.ToLower());
                if(checkUserName != null)
                {
                    TempData["signUp"] = "Username already exist!!";
                    return View(appUser);
                }
                dbContext.AppUsers.Add(appUser);
                await dbContext.SaveChangesAsync();
                HttpContext.Session.SetString("un", appUser.UserName);
                HttpContext.Session.SetString("id", appUser.Password);

                appUser.UserName = null;
                appUser.Password = null;
                TempData["signUp"] = "User registration successfull!!!";
                return RedirectToAction(nameof(Login));

            }
            else
            {
                msg = "Invalid Username or password!!!";
                return View(appUser);
            }
        }
    }
}
