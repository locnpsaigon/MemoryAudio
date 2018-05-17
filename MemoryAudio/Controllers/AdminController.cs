using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using PagedList.Mvc;
using MemoryAudio.Models.Context;
using MemoryAudio.Models.Entities;
using MemoryAudio.Models.Admin;
using MemoryAudio.Models.Security;
using MemoryAudio.Models.Admin.CategoryTree;
using MemoryAudio.Models.Admin.BootstrapTree;
using Newtonsoft.Json;
using MemoryAudio.Libs;

namespace MemoryAudio.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            //EventLogs.Write("Test");
            return View();
        }

        #region Authentication
        //[SSLFilter]
        public ActionResult SignIn()
        {
            var model = new SignInModel();
            model.Username = "";
            model.Password = "";
            model.Remember = false;
            return View(model);
        }

        [HttpPost]
        //[SSLFilter]
        public ActionResult SignIn(SignInModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        var user = db.Users
                            .Where(r => String.Compare(r.Username, model.Username, true) == 0)
                            .FirstOrDefault();
                        if (user == null)
                        {
                            throw new Exception("Tài khoản không hợp lệ");
                        }
                        if (user.Status == 1)
                        {
                            throw new Exception("Tài khoản đang bị khóa");
                        }
                        if (SaltedHash.Verify(user.Salt, user.Password, model.Password) == false)
                        {
                            throw new Exception("Sai mật khẩu");
                        }
                        var authUserData = new AuthUserData();
                        authUserData.UserId = user.UserId;
                        authUserData.FullName = user.FullName;
                        foreach (var role in user.Roles)
                        {
                            authUserData.Roles.Add(role.RoleName);
                        }
                        var ticket = new FormsAuthenticationTicket(
                            1,
                            model.Username,
                            DateTime.Now,
                            DateTime.Now.AddDays(7),
                            model.Remember,
                            JsonConvert.SerializeObject(authUserData));
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                        Response.Cookies.Add(authCookie);

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddUser(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);

            }
            return View(model);
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

        [Authorize]
        public ActionResult UserProfile()
        {

            try
            {
                using (var db = new DBContext())
                {
                    var user = db.Users.Where(r => r.UserId == User.UserId).FirstOrDefault();
                    if (user == null)
                    {
                        throw new Exception("Không tìm thấy thông tin tài khoản!");
                    }
                    var model = new UserProfileModel();
                    model.FullName = user.FullName;
                    model.Phone = user.Phone;
                    model.Email = user.Email;
                    model.CreationDate = user.CreationDate;
                    foreach (var role in user.Roles)
                    {
                        model.Roles.Add(role.RoleName);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - UserProfile: " + ex.ToString());
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult UserProfile(UserProfileModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        var user = db.Users.Where(r => r.UserId == User.UserId).FirstOrDefault();
                        if (user == null)
                        {
                            throw new Exception("Tài khoản không tồn tại!");
                        }
                        if (user.Status == 1)
                        {
                            throw new Exception("Tài khoản bị khóa!");
                        }
                        user.FullName = model.FullName;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - UserProfile: " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // get user info
                        var user = db.Users.Where(r => r.UserId == User.UserId).FirstOrDefault();
                        if (user == null)
                        {
                            throw new Exception("Vui lòng đăng nhập để tiếp tục!");
                        }
                        if (user.Status == 1)
                        {
                            throw new Exception("Tài khoản đang bị khóa!");
                        }
                        if (SaltedHash.Verify(user.Salt, user.Password, model.OldPassword) == false)
                        {
                            throw new Exception("Mật khẩu hiện tại không đúng!");
                        }
                        if (string.Compare(model.OldPassword, model.NewPassword, false) == 0)
                        {
                            throw new Exception("Mật khẩu mới không được trùng với mật khẩu cũ!");
                        }
                        // change password
                        var sh = new SaltedHash(model.NewPassword);
                        user.Salt = sh.Salt;
                        user.Password = sh.Hash;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - ChangePassword: " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        #endregion

        #region Users
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult Users(int status = 0, string filterText = "", int page = 1, int pageSize = 12)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var query = db.Users.AsQueryable();

                    // filtering
                    if (status != 0)
                    {
                        query = query.Where(r => r.Status == status);
                    }
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r =>
                            r.Username.Contains(filterText) ||
                            r.FullName.Contains(filterText) ||
                            r.Phone.Contains(filterText) ||
                            r.Email.Contains(filterText));
                    }
                    // sorting
                    query = query.OrderBy(r => r.Username);

                    // paging
                    var users = query.ToList();
                    var pageCount = users.Count / pageSize;
                    if (users.Count % pageSize > 0) pageCount++;
                    if (page > pageCount) page = pageCount;

                    var model = new UserListViewModel();
                    model.FilterText = filterText;
                    model.Status = status;
                    model.Users = users.ToPagedList<User>(page == 0 ? 1 : page, pageSize);
                    model.PageSize = model.Users.PageSize;
                    model.PageIndex = model.Users.PageNumber;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - Users: " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddUser()
        {
            try
            {
                using (var db = new DBContext())
                {
                    var model = new AddUserModel();
                    model.Status = 2;
                    model.StatusSelector = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "Khóa" },
                        new SelectListItem { Value = "2", Text = "Mở khóa" }
                    };
                    var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                    model.RoleSelector = roles
                        .Select(r => new AddUserModel.RoleSelectorModel
                        {
                            RoleId = r.RoleId,
                            RoleName = r.RoleName,
                            Selected = false
                        })
                        .ToList();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddUser(GET): " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddUser(AddUserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        using (var trans = db.Database.BeginTransaction())
                        {
                            // User existed?
                            var user = db.Users.Where(r => String.Compare(r.Username, model.Username) == 0).FirstOrDefault();
                            if (user != null)
                            {
                                throw new Exception("Tên tài khoản đã được sử dụng. Vui lòng nhập tài khoản khác!");
                            }
                            try
                            {
                                // Create new user
                                user = new User();
                                var saltHash = new SaltedHash(model.Password);
                                user.Username = model.Username;
                                user.Password = saltHash.Hash;
                                user.Salt = saltHash.Salt;
                                user.FullName = model.FullName;
                                user.Phone = model.Phone;
                                user.Email = model.Email;
                                user.CreationDate = DateTime.Now;
                                user.Status = model.Status;
                                db.Users.Add(user);
                                db.SaveChanges();

                                // Assign user's roles
                                foreach (var item in model.RoleSelector)
                                {
                                    if (item.Selected)
                                    {
                                        var role = db.Roles.Where(r => r.RoleId == item.RoleId).FirstOrDefault();
                                        if (role != null)
                                        {
                                            user.Roles.Add(role);
                                        }
                                    }
                                }
                                db.SaveChanges();
                                trans.Commit();

                                return RedirectToAction("Users");
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                throw ex;
                            }
                            //.end try
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddUser(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }
            // Generate status selector
            model.StatusSelector = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Selected = (model.Status == 0), Text = "Khóa" },
                    new SelectListItem { Value = "2", Selected = (model.Status == 1), Text = "Mở khóa" }
                };
            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditUser(int id)
        {
            var model = new EditUserModel();
            try
            {
                using (var db = new DBContext())
                {
                    // Get user info
                    var user = db.Users.Where(r => r.UserId == id).FirstOrDefault();
                    if (user == null)
                    {
                        throw new Exception("Không tìm thấy thông tin tài khoản!");
                    }
                    model.UserId = user.UserId;
                    model.Username = user.Username;
                    model.FullName = user.FullName;
                    model.Phone = user.Phone;
                    model.Email = user.Email;
                    model.Status = user.Status ?? 0;
                    model.StatusSelector = new List<SelectListItem> {
                            new SelectListItem { Value = "1", Selected = (user.Status == 0), Text = "Khóa" },
                            new SelectListItem { Value = "2", Selected = (user.Status == 1), Text = "Mở khóa" }};
                    var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                    model.RoleSelector = roles.Select(r => new EditUserModel.RoleSelectorModel
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Selected = user.IsInRole(r.RoleId)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditUser(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditUser(EditUserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        using (var trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var user = db.Users.Where(r => r.UserId == model.UserId).FirstOrDefault();
                                if (user == null)
                                {
                                    throw new Exception("Không tìm thấy thông tin tài khoản!");
                                }
                                // update password?
                                if (string.IsNullOrWhiteSpace(model.Password) == false)
                                {
                                    var saltHash = new SaltedHash(model.Password);
                                    user.Password = saltHash.Hash;
                                    user.Salt = saltHash.Salt;
                                }
                                user.FullName = model.FullName;
                                user.Phone = model.Phone;
                                user.Email = model.Email;
                                user.Status = model.Status;
                                user.Roles.Clear();
                                foreach (var item in model.RoleSelector)
                                {
                                    if (item.Selected)
                                    {
                                        var role = db.Roles.Where(r => r.RoleId == item.RoleId).FirstOrDefault();
                                        if (role != null)
                                        {
                                            user.Roles.Add(role);
                                        }
                                    }
                                }
                                db.SaveChanges();
                                trans.Commit();

                                return RedirectToAction("Users");
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                throw ex;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditUser(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }
            // Regenerate status selector
            model.StatusSelector = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Selected = (model.Status == 0), Text = "Khóa" },
                    new SelectListItem { Value = "2", Selected = (model.Status == 1), Text = "Mở khóa" }
                };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var user = db.Users.Where(r => r.UserId == id).FirstOrDefault();
                            if (user != null)
                            {
                                // Clear user roles
                                user.Roles.Clear();
                                db.Users.Remove(user);
                                db.SaveChanges();
                                trans.Commit();
                            }
                            return Json(new { Success = true, Message = "Xóa tài khoản thàng công!" });
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteUser: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa tài khoản!" });
            }
        }
        #endregion

        #region Roles 

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult Roles(string filterText = "", int page = 1, int pageSize = 12)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var query = db.Roles.AsQueryable();

                    // Filtering
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r =>
                            r.RoleName.Contains(filterText) ||
                            r.Description.Contains(filterText));
                    }
                    // Sorting
                    query = query.OrderBy(r => r.RoleName);

                    // Paging
                    var roles = query.ToList();
                    var pageCount = roles.Count() / pageSize;
                    if (roles.Count() % pageSize > 0) pageCount++;
                    if (page > pageCount) page = pageCount;

                    var model = new RoleListViewModel();
                    model.FilterText = filterText.Trim();
                    model.Roles = roles.ToPagedList<Role>(page == 0 ? 1 : page, pageSize);
                    model.PageSize = model.Roles.PageSize;
                    model.PageIndex = model.Roles.PageNumber;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - Roles: " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddRole()
        {
            try
            {
                var model = new AddRoleModel();
                return View(model);
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddUser(GET): " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddRole(AddRoleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // Role existed?
                        var role = db.Roles.Where(r => String.Compare(r.RoleName, model.RoleName) == 0).FirstOrDefault();
                        if (role != null)
                        {
                            throw new Exception("Chức danh đã được sử dung. Vui lòng nhập tên chức danh khác!");
                        }
                        // Create new user
                        role = new Role();
                        role.RoleName = model.RoleName;
                        role.Description = model.Description;
                        db.Roles.Add(role);
                        db.SaveChanges();

                        return RedirectToAction("Roles");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddUser(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditRole(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var model = new EditRoleModel();

                    // Get role info
                    var role = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                    if (role == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy chức danh!");
                        return View(model);
                    }
                    model.RoleId = role.RoleId;
                    model.RoleName = role.RoleName;
                    model.Description = role.Description;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditUser(GET): " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }

        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditRole(EditRoleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // Role existed?
                        var role = db.Roles.Where(r => r.RoleId == model.RoleId).FirstOrDefault();
                        if (role == null)
                        {
                            throw new Exception("Không tìm thấy thông tin chức danh");
                        }

                        // Role name duplicated?
                        var roleDuplicated = db.Roles.Where(r => r.RoleId != model.RoleId && string.Compare(r.RoleName, model.RoleName, true) == 0).FirstOrDefault();
                        if (roleDuplicated != null)
                        {
                            throw new Exception("Tên chức danh đã được sử dụng. Vui lòng nhập tên chức danh khác!");
                        }

                        role.RoleName = model.RoleName;
                        role.Description = model.Description;
                        db.SaveChanges();

                        return RedirectToAction("Roles");
                    }
                }

            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditUser(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteRole(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var role = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                            if (role != null)
                            {
                                // Clear user roles
                                role.Users.Clear();
                                db.Roles.Remove(role);
                                db.SaveChanges();
                                trans.Commit();
                            }
                            return Json(new { Success = true, Message = "Xóa chức danh thàng công!" });
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteRole: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa chức danh!" });
            }
        }
        #endregion

        #region Categories

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult Categories()
        {
            return View();
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddCategory(int parentId = 0)
        {
            var model = new AddCategoryModel();
            try
            {
                using (var db = new DBContext())
                {
                    var categoryNodes = CategoryTree.GetCategoryTree().ToList();
                    model.ParentSelecor = categoryNodes
                        .Select(r => new SelectListItem
                        {
                            Value = r.CategoryId.ToString(),
                            Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName,
                            Selected = (r.CategoryId == parentId)
                        })
                        .ToList();
                    model.SortIdx = db.Categories.Max(r => r.SortIdx) + 1;

                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddCategory: " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddCategory(AddCategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // category name existed?
                        var nameExisted = db.Categories.Where(r => r.ParentId == model.ParentId && string.Compare(r.CategoryName, model.CategoryName, true) == 0).FirstOrDefault() != null;
                        if (nameExisted)
                        {
                            throw new Exception("Tên danh mục đã được sử dụng. Vui lòng nhập tên khác!");
                        }
                        var newCategory = new Category();
                        newCategory.CategoryName = model.CategoryName;
                        newCategory.Description = model.Description;
                        newCategory.ParentId = model.ParentId;
                        newCategory.SortIdx = model.SortIdx;
                        db.Categories.Add(newCategory);
                        db.SaveChanges();
                        return RedirectToAction("Categories");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                EventLogs.Write("AdminController - AddCategory(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }

            // Regenerate category selector
            try
            {
                using (var db = new DBContext())
                {
                    var categoryNodes = CategoryTree.GetCategoryTree().ToList();
                    model.ParentSelecor = categoryNodes
                        .Select(r => new SelectListItem
                        {
                            Value = r.CategoryId.ToString(),
                            Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName,
                            Selected = (r.CategoryId == model.ParentId)
                        })
                        .ToList();

                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddCategory(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditCategory(int id)
        {
            var model = new EditCategoryModel();
            try
            {
                using (var db = new DBContext())
                {
                    // Get category info
                    var category = db.Categories.Where(r => r.CategoryId == id).FirstOrDefault();
                    if (category == null)
                    {
                        throw new Exception("Không tìm thấy thông tin danh mục hàng hóa cần chỉnh sửa");
                    }
                    // Assign model values
                    model.CategoryId = category.CategoryId;
                    model.CategoryName = category.CategoryName;
                    model.Description = category.Description;
                    model.ParentId = category.ParentId;
                    model.SortIdx = category.SortIdx;
                    // Generate parent category selector
                    var categoryNodes = CategoryTree.GetCategoryTree().ToList();
                    model.ParentSelecor = categoryNodes
                        .Select(r => new SelectListItem
                        {
                            Value = r.CategoryId.ToString(),
                            Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName,
                            Selected = (r.CategoryId == category.ParentId)
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditCategory(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditCategory(EditCategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // Get category info
                        var category = db.Categories.Where(r => r.CategoryId == model.CategoryId).FirstOrDefault();
                        if (category == null)
                        {
                            throw new Exception("Không tìm thấy thông tin danh mục sản phẩm cần cập nhật");
                        }

                        // Category name existed
                        var categoryNameExisted = db.Categories.Where(r => r.CategoryId != model.CategoryId && string.Compare(r.CategoryName, model.CategoryName, true) == 0).FirstOrDefault() != null;
                        if (categoryNameExisted)
                        {
                            throw new Exception("Tên danh mục sản phẩm đã được sử dụng. Vui lòng nhập tên khác!");
                        }

                        // Update category info
                        category.CategoryName = model.CategoryName;
                        category.Description = model.Description;
                        category.ParentId = model.ParentId;
                        category.SortIdx = model.SortIdx;
                        db.SaveChanges();

                        return RedirectToAction("Categories");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditCategory(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            // Regenerate category selector
            try
            {
                using (var db = new DBContext())
                {
                    var categoryNodes = CategoryTree.GetCategoryTree().ToList();
                    model.ParentSelecor = categoryNodes
                        .Select(r => new SelectListItem
                        {
                            Value = r.CategoryId.ToString(),
                            Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName,
                            Selected = (r.CategoryId == model.ParentId)
                        })
                        .ToList();

                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditCategory(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var category = db.Categories.Where(r => r.CategoryId == id).FirstOrDefault();
                    if (category == null)
                    {
                        return Json(new { Success = false, Message = "Xóa danh mục sản phẩm thất bại! Không tìm thấy danh mục sản phẩm cần xóa" });
                    }
                    var childCategory = db.Categories.Where(r => r.ParentId == category.CategoryId).FirstOrDefault();
                    if (childCategory != null)
                    {
                        return Json(new { Success = false, Message = "Xóa danh mục sản phẩm thất bại! Bạn chỉ được xóa danh mục sản phẩm không có danh mục con" });
                    }
                    var categoryProduct = db.Products.Where(r => r.CategoryId == category.CategoryId).FirstOrDefault();
                    if (categoryProduct != null)
                    {
                        return Json(new { Success = false, Message = "Xóa danh mục sản phẩm thất bại! Bạn chỉ được phép xóa danh mục con rỗng và không có sản phẩm phụ thuộc nào." });
                    }
                    db.Categories.Remove(category);
                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Xóa danh mục sản phẩm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteCategory: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa danh mục sản phẩm!" });
            }
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult GetCategoryBootstrapTree()
        {
            try
            {
                var bsTree = BootstrapTree.LoadCategoryBootstrapTree();
                return Json(new
                {
                    Success = true,
                    Message = "Nạp cây danh mục sản phẩm thành công!",
                    Data = bsTree.nodes
                });
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - GetCategoriesTree: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình nạp danh mục sản phẩm!" });
            }
        }

        [HttpPost]
        public JsonResult MoveCategoryUp(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    // get category info
                    var category = db.Categories.Where(r => r.CategoryId == id).FirstOrDefault();
                    if (category == null)
                    {
                        throw new Exception("Không tìm thấy danh mục cần di chuyển!");
                    }
                    var categoryAbove = db.Categories
                        .Where(r => r.SortIdx < category.SortIdx &&
                               r.CategoryId != category.CategoryId &&
                               r.ParentId == category.ParentId)
                        .OrderByDescending(r => r.SortIdx)
                        .Take(1)
                        .FirstOrDefault();
                    if (categoryAbove != null)
                    {
                        // swap sortindex
                        var sortIdxTemp = categoryAbove.SortIdx;
                        categoryAbove.SortIdx = category.SortIdx;
                        category.SortIdx = sortIdxTemp;
                        db.SaveChanges();
                    }
                    return Json(new { Success = true, Message = "Sắp xếp danh mục thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - MoveCategoryUp: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình sắp xếp danh mục!" });
            }
        }

        [HttpPost]
        public JsonResult MoveCategoryDown(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    // get category info
                    var category = db.Categories.Where(r => r.CategoryId == id).FirstOrDefault();
                    if (category == null)
                    {
                        throw new Exception("Không tìm thấy danh mục cần di chuyển!");
                    }
                    var categoryAbove = db.Categories
                        .Where(r => r.SortIdx > category.SortIdx &&
                               r.CategoryId != category.CategoryId &&
                               r.ParentId == category.ParentId)
                        .OrderBy(r => r.SortIdx)
                        .Take(1)
                        .FirstOrDefault();
                    if (categoryAbove != null)
                    {
                        // swap sortindex
                        var sortIdxTemp = categoryAbove.SortIdx;
                        categoryAbove.SortIdx = category.SortIdx;
                        category.SortIdx = sortIdxTemp;
                        db.SaveChanges();
                    }
                    return Json(new { Success = true, Message = "Sắp xếp danh mục thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - MoveCategoryDown: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình sắp xếp danh mục!" });
            }
        }

        #endregion

        #region Brands
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult Brands(string filterText = "", int page = 1, int pageSize = 12)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var query = db.Brands.AsQueryable();

                    // filtering
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r =>
                            r.BrandName.Contains(filterText) ||
                            r.Description.Contains(filterText));
                    }
                    // sorting
                    query = query.OrderBy(r => r.BrandName);

                    // Paging
                    var brands = query.ToList();
                    var pageCount = brands.Count / pageSize;
                    if (brands.Count % pageSize > 0) pageCount++;
                    if (page > pageCount) page = pageCount;

                    var model = new BrandListViewModel();
                    model.FilterText = filterText.Trim();
                    model.Brands = brands.ToPagedList<Brand>(page == 0 ? 1 : page, pageSize);
                    model.PageSize = model.Brands.PageSize;
                    model.PageIndex = model.Brands.PageNumber;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - Brands: " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddBrand()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddBrand(AddBrandModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // brand name existed?
                        var nameExisted = db.Brands.Where(r => string.Compare(r.BrandName, model.BrandName, true) == 0).FirstOrDefault() != null;
                        if (nameExisted)
                        {
                            throw new Exception("Tên thương hiệu sản phẩm đã được sử dụng. Vui lòng nhập tên khác!");
                        }
                        var newBrand = new Brand();
                        newBrand.BrandName = model.BrandName;
                        newBrand.Description = model.Description;
                        db.Brands.Add(newBrand);
                        db.SaveChanges();
                        return RedirectToAction("Brands");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                EventLogs.Write("AdminController - AddBrand(POST): " + ex.ToString());

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditBrand(int id)
        {
            var model = new EditBrandModel();
            try
            {
                using (var db = new DBContext())
                {
                    // Get category info
                    var brand = db.Brands.Where(r => r.BrandId == id).FirstOrDefault();
                    if (brand == null)
                    {
                        throw new Exception("Không tìm thấy thông tin thương hiệu sản phẩm cần chỉnh sửa");
                    }
                    // Assign model values
                    model.BrandId = brand.BrandId;
                    model.BrandName = brand.BrandName;
                    model.Description = brand.Description;
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditBrand(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditBrand(EditBrandModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new DBContext())
                    {
                        // Get category info
                        var brand = db.Brands.Where(r => r.BrandId == model.BrandId).FirstOrDefault();
                        if (brand == null)
                        {
                            throw new Exception("Không tìm thấy thông tin thương hiệu sản phẩm cần cập nhật");
                        }

                        // Category name existed
                        var brandNameExisted = db.Brands.Where(r => r.BrandId != model.BrandId && string.Compare(r.BrandName, model.BrandName, true) == 0).FirstOrDefault() != null;
                        if (brandNameExisted)
                        {
                            throw new Exception("Tên thương hiệu sản phẩm đã được sử dụng. Vui lòng nhập tên khác!");
                        }

                        // Update brand info
                        brand.BrandName = model.BrandName;
                        brand.Description = model.Description;
                        db.SaveChanges();

                        return RedirectToAction("Brands");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditCategory(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteBrand(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var brand = db.Brands.Where(r => r.BrandId == id).FirstOrDefault();
                    if (brand == null)
                    {
                        throw new Exception("Xóa thương hiệu thất bại! Không tìm thấy thương hiệu sản phẩm cần xóa!");
                    }
                    // var brandProductsExisted = db....
                    var brandProductsExisted = false;
                    if (brandProductsExisted)
                    {
                        throw new Exception("Xóa thương hiệu thất bại! Bạn chỉ được xóa thương hiệu sản phẩm không có sản phẩm con");
                    }

                    db.Brands.Remove(brand);
                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Xóa thương hiệu sản phẩm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteCategory: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa thương hiệu sản phẩm!" });
            }
        }
        #endregion

        #region Products
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult Products(string filterText = "", int categoryId = 0, int brandId = 0, int display = 0, string sortOrder = "", int page = 1, int pageSize = 12)
        {
            try
            {
                // query products
                using (var db = new DBContext())
                {
                    var query = from p in db.Products
                                join c in db.Categories on p.CategoryId equals c.CategoryId into pc
                                join b in db.Brands on p.BrandId equals b.BrandId into pb
                                join d in db.DisplayStatuses on p.Display equals d.Id into pd
                                from j1 in pc.DefaultIfEmpty()
                                from j2 in pb.DefaultIfEmpty()
                                from j3 in pd.DefaultIfEmpty()
                                select new ProductInfo
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    CategoryId = p.CategoryId,
                                    CategoryName = j1.CategoryName,
                                    BrandId = p.BrandId,
                                    BrandName = j2.BrandName,
                                    Specification = p.Specification,
                                    TotalInStock = p.TotalInStock,
                                    Price = p.Price,
                                    Discount = p.Discount,
                                    Image1 = p.Image1,
                                    Image2 = p.Image2,
                                    Image3 = p.Image3,
                                    Image4 = p.Image4,
                                    Image5 = p.Image5,
                                    Image6 = p.Image6,
                                    CreationDate = p.CreationDate,
                                    Display = p.Display,
                                    DisplayName = j3.Name,
                                    SortIdx = p.SortIdx
                                };
                    // Filtering 
                    if (categoryId > 0)
                    {
                        // Filter by category
                        var category = db.Categories.Where(r => r.CategoryId == categoryId).FirstOrDefault();
                        if (category != null)
                        {
                            // Create parent node
                            var parent = new CategoryTreeNode();
                            parent.CategoryId = category.CategoryId;
                            parent.CategoryName = category.CategoryName;
                            parent.Description = category.Description;
                            parent.Level = 0;
                            parent.Parent = null;
                            parent.Nodes = new List<CategoryTreeNode>();
                            CategoryTree.AppendChildNodes(parent);
                            var childNodes = parent.GetChildNodes();
                            var childCategoryIds = new List<int>();
                            foreach (var node in childNodes)
                            {
                                childCategoryIds.Add(node.CategoryId);
                            }
                            if (childCategoryIds.Count > 0)
                            {
                                query = query.Where(r => childCategoryIds.Contains(r.CategoryId ?? 0));
                            }
                        }
                    }
                    if (brandId > 0)
                    {
                        // Filter by brand
                        query = query.Where(r => r.BrandId == brandId);
                    }
                    if (display > 0)
                    {
                        // Filter by display
                        query = query.Where(r => r.Display == display);
                    }
                    if (string.IsNullOrWhiteSpace(filterText) == false)
                    {
                        // Fiter by name
                        query = query.Where(r =>
                                            r.ProductName.Contains(filterText) ||
                                            r.CategoryName.Contains(filterText) ||
                                            r.BrandName.Contains(filterText));
                    }
                    // Sorting
                    switch (sortOrder)
                    {
                        case "price":
                            query = query.OrderBy(p => p.Price);
                            break;

                        case "price_desc":
                            query = query.OrderByDescending(p => p.Price);
                            break;

                        case "name":
                            query = query.OrderBy(p => p.ProductName);
                            break;

                        case "name_desc":
                            query = query.OrderByDescending(p => p.ProductName);
                            break;

                        default:
                            query = query.OrderByDescending(p => p.SortIdx);
                            break;
                    }

                    // Paging
                    var products = query.ToList();
                    var pageCount = products.Count() / pageSize;
                    if (products.Count() % pageSize > 0) pageCount++;
                    if (page > pageCount) page = pageCount;

                    // Crate view model
                    var model = new ProductListViewModel();
                    model.FilterText = filterText;
                    model.CategoryId = categoryId;
                    model.BrandId = brandId;
                    model.Display = display;
                    model.SortOrder = sortOrder;
                    model.Products = query.ToPagedList<ProductInfo>(page == 0 ? 1 : page, pageSize);
                    model.PageIndex = model.Products.PageNumber;
                    model.PageSize = model.PageSize;

                    /*
                     * CREATE OPTION SELECTORS
                     */
                    // Create category selector
                    model.CategorySelector = CategoryTree.GetCategoryTree().ToList()
                        .Select(r => new SelectListItem { Value = r.CategoryId.ToString(), Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName, Selected = (r.CategoryId == categoryId) })
                        .ToList();

                    // Create brand selector
                    model.BrandSelector = db.Brands.OrderBy(r => r.BrandName)
                        .Select(r => new SelectListItem
                        {
                            Value = r.BrandId.ToString(),
                            Text = r.BrandName,
                            Selected = (r.BrandId == brandId)
                        }).ToList();

                    // Create brand selector
                    model.DisplaySelector = db.DisplayStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem
                        {
                            Value = r.Id.ToString(),
                            Text = r.Name,
                            Selected = (r.Id == display)
                        }).ToList();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - Products: " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddProduct(int categoryId = 0)
        {
            var model = new AddProductModel();
            try
            {
                using (var db = new DBContext())
                {
                    model.Price = "0";
                    model.Discount = "0";
                    model.TotalInStock = "1";
                    model.Display = 1;

                    // Generate category selector
                    var categoryNodes = CategoryTree.GetCategoryTree().ToList();
                    model.CategorySelector = categoryNodes
                        .Select(r => new SelectListItem
                        {
                            Value = r.CategoryId.ToString(),
                            Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName,
                            Selected = (r.CategoryId == categoryId)
                        })
                        .ToList();

                    // Generate brand selector
                    model.BrandSelector = db.Brands.OrderBy(r => r.BrandName)
                        .Select(r => new SelectListItem { Value = r.BrandId.ToString(), Text = r.BrandName })
                        .ToList();

                    // Generate display selector
                    model.DisplaySelector = db.DisplayStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                        .ToList();

                    model.SortIdx = (db.Products.Max(r => r.SortIdx) + 1).ToString();
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddProduct(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddProduct(AddProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate inputs
                    var price = (decimal)0;
                    var discount = (decimal)0;
                    var quantity = 0;
                    var sortidx = 1000;
                    if (decimal.TryParse(model.Price.Replace(",", ""), out price) == false)
                    {
                        throw new Exception("Giá sản phẩm không hợp lệ! Giá sản phẩm phải là số.");
                    }
                    if (decimal.TryParse(model.Discount.Replace(",", ""), out discount) == false)
                    {
                        throw new Exception("Giảm giá sản phẩm không hợp lệ! Giảm giá sản phẩm phải là số.");
                    }
                    if (int.TryParse(model.TotalInStock.Replace(",", ""), out quantity) == false)
                    {
                        throw new Exception("Số lượng tồn không hợp lệ! Số lượng tồn phải là số.");
                    }
                    if (int.TryParse(model.SortIdx.Replace(",", ""), out sortidx) == false)
                    {
                        throw new Exception("Thứ tự sắp xếp không hợp lệ! Thứ tự sắp xếp phải là số.");
                    }

                    var newProduct = new Product();
                    newProduct.CategoryId = model.CategoryId;
                    newProduct.BrandId = model.BrandId;
                    newProduct.ProductName = model.ProductName;
                    newProduct.Specification = model.Specification;
                    newProduct.TotalInStock = quantity;
                    newProduct.Price = price;
                    newProduct.Discount = discount;
                    newProduct.Image1 = model.Image1;
                    newProduct.Image2 = model.Image2;
                    newProduct.Image3 = model.Image3;
                    newProduct.Image4 = model.Image4;
                    newProduct.Image5 = model.Image5;
                    newProduct.Image6 = model.Image6;
                    newProduct.CreationDate = DateTime.Now;
                    newProduct.Display = model.Display;
                    newProduct.SortIdx = sortidx;
                    using (var db = new DBContext())
                    {
                        db.Products.Add(newProduct);
                        db.SaveChanges();
                        return RedirectToAction("Products", new { categoryId = newProduct.CategoryId });
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - AddProduct(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            // Generate selectors
            using (var db = new DBContext())
            {
                try
                {
                    // Generate category selector
                    model.CategorySelector = CategoryTree.GetCategoryTree().ToList()
                        .Select(r => new SelectListItem { Value = r.CategoryId.ToString(), Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName, Selected = (r.CategoryId == model.CategoryId) })
                        .ToList();

                    // Generate brand selector
                    model.BrandSelector = db.Brands.OrderBy(r => r.BrandName)
                        .Select(r => new SelectListItem { Value = r.BrandId.ToString(), Text = r.BrandName, Selected = (r.BrandId == model.BrandId) })
                        .ToList();

                    // Generate display selector
                    model.DisplaySelector = db.DisplayStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Display) })
                        .ToList();
                }
                catch (Exception ex)
                {
                    // Write error log
                    EventLogs.Write("AdminController - AddProduct: " + ex.ToString());
                }
            }
            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditProduct(int id)
        {
            var model = new EditProductModel();
            try
            {
                using (var db = new DBContext())
                {
                    var product = db.Products.Where(r => r.ProductId == id).FirstOrDefault();
                    if (product == null)
                    {
                        throw new Exception("Không tìm thấy thông tin sản phẩm cần cập nhật!");
                    }
                    model.ProductId = product.ProductId;
                    model.CategoryId = product.CategoryId ?? 0;
                    model.BrandId = product.BrandId ?? 0;
                    model.ProductName = product.ProductName;
                    model.Specification = product.Specification;
                    model.Price = product.Price.ToString();
                    model.Discount = product.Discount.ToString();
                    model.Image1 = product.Image1;
                    model.Image2 = product.Image2;
                    model.Image3 = product.Image3;
                    model.Image4 = product.Image4;
                    model.Image5 = product.Image5;
                    model.Image6 = product.Image6;
                    model.Display = product.Display;
                    model.SortIdx = product.SortIdx.ToString();

                    // Create Category selector
                    model.CategorySelector = CategoryTree.GetCategoryTree().ToList()
                       .Select(r => new SelectListItem { Value = r.CategoryId.ToString(), Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName, Selected = (r.CategoryId == product.CategoryId) })
                       .ToList();
                    model.TotalInStock = product.TotalInStock.ToString();
                    // Create Brand selector
                    model.BrandSelector = db.Brands.OrderBy(r => r.BrandName)
                       .Select(r => new SelectListItem { Value = r.BrandId.ToString(), Text = r.BrandName, Selected = (r.BrandId == product.BrandId) })
                       .ToList();
                    // Generate display selector
                    model.DisplaySelector = db.DisplayStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == product.Display) })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditProduct(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditProduct(EditProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate inputs
                    var price = (decimal)0;
                    var discount = (decimal)0;
                    var quantity = 0;
                    var sortidx = 1000;
                    if (decimal.TryParse(model.Price.Replace(",", ""), out price) == false)
                    {
                        throw new Exception("Giá sản phẩm không hợp lệ! Giá sản phẩm phải là số.");
                    }
                    if (decimal.TryParse(model.Discount.Replace(",", ""), out discount) == false)
                    {
                        throw new Exception("Giảm giá sản phẩm không hợp lệ! Giảm giá sản phẩm phải là số.");
                    }
                    if (int.TryParse(model.TotalInStock.Replace(",", ""), out quantity) == false)
                    {
                        throw new Exception("Số lượng tồn không hợp lệ! Số lượng tồn phải là số.");
                    }
                    if (int.TryParse(model.SortIdx.Replace(",", ""), out sortidx) == false)
                    {
                        throw new Exception("Thứ tự sắp xếp không hợp lệ! Thứ tự sắp xếp phải là số.");
                    }
                    // Get product
                    using (var db = new DBContext())
                    {
                        var product = db.Products.Where(r => r.ProductId == model.ProductId).FirstOrDefault();
                        if (product == null)
                        {
                            throw new Exception("Không tìm thấy thông tin sản phẩm cần cập nhật!");
                        }
                        // Update product info
                        product.CategoryId = model.CategoryId;
                        product.BrandId = model.BrandId;
                        product.ProductName = model.ProductName;
                        product.Specification = model.Specification;
                        product.TotalInStock = quantity;
                        product.Price = price;
                        product.Discount = discount;
                        product.Image1 = model.Image1;
                        product.Image2 = model.Image2;
                        product.Image3 = model.Image3;
                        product.Image4 = model.Image4;
                        product.Image5 = model.Image5;
                        product.Image6 = model.Image6;
                        product.CreationDate = DateTime.Now;
                        product.Display = model.Display;
                        product.SortIdx = sortidx;
                        db.SaveChanges();
                        return RedirectToAction("Products", new { categoryId = product.CategoryId });
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditProduct(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            // Generate selectors
            using (var db = new DBContext())
            {
                try
                {
                    // Create category selector
                    model.CategorySelector = CategoryTree.GetCategoryTree().ToList()
                        .Select(r => new SelectListItem { Value = r.CategoryId.ToString(), Text = new string('\xA0', r.Level * 4) + " " + r.CategoryName, Selected = (r.CategoryId == model.CategoryId) })
                        .ToList();
                    // Create Brand selector
                    model.BrandSelector = db.Brands.OrderBy(r => r.BrandName)
                       .Select(r => new SelectListItem { Value = r.BrandId.ToString(), Text = r.BrandName, Selected = (r.BrandId == model.BrandId) })
                       .ToList();
                    // Generate display selector
                    model.DisplaySelector = db.DisplayStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Display) })
                        .ToList();
                }
                catch (Exception ex)
                {
                    // Write error log
                    EventLogs.Write("AdminController - AddProduct: " + ex.ToString());
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteProduct(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var product = db.Products.Where(r => r.ProductId == id).FirstOrDefault();
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                    }
                    return Json(new { Success = true, Message = "Xóa sản phẩm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteProduct: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa sản phẩm!" });
            }
        }
        #endregion

        #region News
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult News(string filterText = "", int type = 0, int status = 0, int page = 1, int pageSize = 12)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var query = db.News.AsQueryable();

                    // filtering
                    if (type != 0)
                    {
                        query = query.Where(r => r.Type == type);
                    }
                    if (status != 0)
                    {
                        query = query.Where(r => r.Status == status);
                    }

                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        query = query.Where(r =>
                            r.Title.Contains(filterText) ||
                            r.Tags.Contains(filterText));
                    }
                    // sorting
                    query = query.OrderByDescending(r => r.ReleaseDate);

                    // Paging
                    var news = query.ToList();
                    var pageCount = news.Count / pageSize;
                    if (news.Count % pageSize > 0) pageCount++;
                    if (page > pageCount) page = pageCount;

                    var model = new NewsListViewModel();
                    model.FilterText = filterText.Trim();
                    model.Type = type;
                    model.Status = status;
                    // Create type/status selector options
                    model.TypeSelector = db.NewsTypes.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == type) })
                        .ToList();
                    model.StatusSelector = db.NewsStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == status) })
                        .ToList();
                    model.News = news.ToPagedList<News>(page == 0 ? 1 : page, pageSize);
                    model.PageSize = model.News.PageSize;
                    model.PageIndex = model.News.PageNumber;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - News: " + ex.ToString());
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult AddNews()
        {
            var model = new AddNewsModel();
            model.ReleaseDate = DateTime.Now.ToString("dd/MM/yyyy");
            using (var db = new DBContext())
            {
                // Create type/status selector options
                model.TypeSelector = db.NewsTypes.OrderBy(r => r.Name)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToList();
                model.StatusSelector = db.NewsStatuses.OrderBy(r => r.Name)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToList();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        [ValidateInput(false)]
        public ActionResult AddNews(AddNewsModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate input
                    var releaseDate = DateTime.Now;
                    if (DateTime.TryParseExact(model.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate) == false)
                    {
                        throw new Exception("Thời gian phát hành không hợp lệ! Phải đúng định dạng (dd/MM/yyyy)");
                    }

                    using (var db = new DBContext())
                    {
                        var news = new News();
                        news.Title = model.Title;
                        news.Icon = model.Icon;
                        news.Lead = model.Lead;
                        news.Body = model.Body;
                        news.Tags = model.Tags;
                        news.Type = model.Type;
                        news.Status = model.Status;
                        news.ReleaseDate = releaseDate;
                        db.News.Add(news);
                        db.SaveChanges();
                        return RedirectToAction("News");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                EventLogs.Write("AdminController - AddNewsModel(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                using(var db = new DBContext())
                {
                    // Create type/status selector options
                    model.TypeSelector = db.NewsTypes.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Type) })
                        .ToList();
                    model.StatusSelector = db.NewsStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Status) })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                EventLogs.Write("AdminController - AddNewsModel(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            
            return View(model);
        }

        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public ActionResult EditNews(int id)
        {
            var model = new EditNewsModel();
            try
            {
                using (var db = new DBContext())
                {
                    // Get category info
                    var news = db.News.Where(r => r.NewsId == id).FirstOrDefault();
                    if (news == null)
                    {
                        throw new Exception("Không tìm thấy bài viết cần chỉnh sửa");
                    }
                    // Assign model values
                    model.NewsId = news.NewsId;
                    model.Title = news.Title;
                    model.Icon = news.Icon;
                    model.Lead = news.Lead;
                    model.Body = news.Body;
                    model.Tags = news.Tags;
                    model.Type = news.Type;
                    model.Status = news.Status;
                    model.ReleaseDate = news.ReleaseDate.ToString("dd/MM/yyyy");
                    // Create type/status selectors
                    // Create type/status selector options
                    model.TypeSelector = db.NewsTypes.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == news.Type) })
                        .ToList();
                    model.StatusSelector = db.NewsStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == news.Status) })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditNews(GET): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        [ValidateInput(false)]
        public ActionResult EditNews(EditNewsModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate input
                    var releaseDate = DateTime.Now;
                    if (DateTime.TryParseExact(model.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate) == false)
                    {
                        throw new Exception("Thời gian phát hành không hợp lệ! Phải đúng định dạng (dd/MM/yyyy)");
                    }

                    using (var db = new DBContext())
                    {
                        // Get category info
                        var news = db.News.Where(r => r.NewsId == model.NewsId).FirstOrDefault();
                        if (news == null)
                        {
                            throw new Exception("Không tìm thấy bài viết cần cập nhật");
                        }
                        // Update brand info
                        news.Title = model.Title;
                        news.Icon = model.Icon;
                        news.Lead = model.Lead;
                        news.Body = model.Body;
                        news.Tags = model.Tags;
                        news.Type = model.Type;
                        news.Status = model.Status;
                        news.ReleaseDate = releaseDate;
                        db.SaveChanges();
                        return RedirectToAction("News");
                    }
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - EditCategory(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            try
            {
                using (var db = new DBContext())
                {
                    // Create type/status selector options
                    model.TypeSelector = db.NewsTypes.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Type) })
                        .ToList();
                    model.StatusSelector = db.NewsStatuses.OrderBy(r => r.Name)
                        .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name, Selected = (r.Id == model.Status) })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                EventLogs.Write("AdminController - EditCategory(POST): " + ex.ToString());
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [AdminAuthorize(Roles = "Administrators")]
        public JsonResult DeleteNews(int id)
        {
            try
            {
                using (var db = new DBContext())
                {
                    var news = db.News.Where(r => r.NewsId == id).FirstOrDefault();
                    if (news == null)
                    {
                        throw new Exception("Xóa bài viết thất bại! Không tìm thấy bài viết cần xóa!");
                    }
                    db.News.Remove(news);
                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Xóa bài viết thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                EventLogs.Write("AdminController - DeleteNews: " + ex.ToString());

                return Json(new { Success = false, Message = "Có lỗi xảy ra trong quá trình xóa bài viết!" });
            }
        }
        #endregion
    }
}