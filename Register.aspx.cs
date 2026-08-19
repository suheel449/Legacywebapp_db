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

        if (UserStore.EmailExists(email))
        {
            ShowMessage("An account with this email already exists.", true);
            return;
        }

        UserStore.Register(name, email, password);
        Session["UserEmail"] = email.Trim().ToLowerInvariant();
        Session["UserName"] = name;
        Response.Redirect("Dashboard.aspx");
    }

    private void ShowMessage(string text, bool isError)
    {
        lblMessage.Visible = true;
        lblMessage.Text = text;
        lblMessage.CssClass = isError ? "message error" : "message success";
    }
}
