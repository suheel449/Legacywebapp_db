using System;
using System.Web.UI;

public partial class RegisterPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserEmail"] != null)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string name = (txtName.Text ?? string.Empty).Trim();
        string email = (txtEmail.Text ?? string.Empty).Trim();
        string password = txtPassword.Text ?? string.Empty;
        string confirm = txtConfirm.Text ?? string.Empty;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please fill in all fields.", true);
            return;
        }

        if (password.Length < 4)
        {
            ShowMessage("Password must be at least 4 characters.", true);
            return;
        }

        if (password != confirm)
        {
            ShowMessage("Passwords do not match.", true);
            return;
        }

        try
        {
            if (AppDb.EmailExists(email))
            {
                ShowMessage("An account with this email already exists.", true);
                return;
            }

            AppUser user = AppDb.Register(name, email, password);
            Session["UserId"] = user.UserId;
            Session["UserEmail"] = user.Email;
            Session["UserName"] = user.Name;
            Session["UserRole"] = user.RoleName;
            Response.Redirect("Dashboard.aspx");
        }
        catch (Exception)
        {
            ShowMessage("Unable to reach the database. Check the connection string and that LegacyBizDb exists.", true);
        }
    }

    private void ShowMessage(string text, bool isError)
    {
        lblMessage.Visible = true;
        lblMessage.Text = text;
        lblMessage.CssClass = isError ? "message error" : "message success";
    }
}
