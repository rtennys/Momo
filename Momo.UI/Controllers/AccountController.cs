using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Mvc;
using Microsoft.Web.WebPages.OAuth;
using Momo.Common.DataAccess;
using Momo.Domain;
using Momo.Domain.Commands;
using Momo.UI.Models;
using WebMatrix.WebData;

namespace Momo.UI.Controllers
{
    [Authorize]
    public class AccountController : AppController
    {
        public AccountController(IUnitOfWork uow, ICommandExecutor commandExecutor)
        {
            _uow = uow;
            _commandExecutor = commandExecutor;
        }

        private readonly IUnitOfWork _uow;
        private readonly ICommandExecutor _commandExecutor;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginModel {RememberMe = true});
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.Username, model.Password, persistCookie: model.RememberMe))
                return RedirectToLocal(returnUrl);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _commandExecutor.Execute((AddUserCommand)model);

                if (result.AnyErrors())
                    ModelState.AddModelErrors(result);
                else
                {
                    _uow.Commit();

                    WebSecurity.CreateAccount(model.Username, model.Password);
                    WebSecurity.Login(model.Username, model.Password);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            var ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {IsolationLevel = IsolationLevel.Serializable}))
                {
                    var hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new {Message = message});
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                          : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            var hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                        return RedirectToAction("Manage", new {Message = ManageMessageId.ChangePasswordSuccess});

                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                    state.Errors.Clear();

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new {Message = ManageMessageId.SetPasswordSuccess});
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new {ReturnUrl = returnUrl}));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new {ReturnUrl = returnUrl}));
            if (!result.IsSuccessful)
                return RedirectToAction("ExternalLoginFailure");

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: true))
                return RedirectToLocal(returnUrl);

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }

            // User is new, ask for their desired membership name
            var loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", new RegisterExternalLoginModel {Username = ParseUsername(result.UserName), ExternalLoginData = loginData});
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
                return RedirectToAction("Manage");

            if (ModelState.IsValid)
            {
                var result = _commandExecutor.Execute((AddUserCommand)model);

                if (result.AnyErrors())
                    ModelState.AddModelErrors(result);
                else
                {
                    _uow.Commit();

                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.Username);
                    OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: true);

                    return RedirectToLocal(returnUrl);
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous, ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            var accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = new List<ExternalLogin>();
            foreach (var account in accounts)
            {
                var clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                                   {
                                       Provider = account.Provider,
                                       ProviderDisplayName = clientData.DisplayName,
                                       ProviderUserId = account.ProviderUserId,
                                   });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));

            return PartialView("_RemoveExternalLoginsPartial", externalLogins.ToArray());
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        private string ParseUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return "";

            if (username.IndexOf('@') >= 0) username = username.Substring(0, username.IndexOf('@'));
            if (username.IndexOf('.') >= 0) username = username.Substring(0, username.IndexOf('.'));

            return Regex.Replace(username, "[^A-Za-z0-9-]", "-");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }
    }
}
