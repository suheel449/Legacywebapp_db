using System;
using System.Web.UI;

public partial class LoginPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserEmail"] != null)
        {
            Response.Redirect("Dashboard.aspx");
            return;
        }

        if (!IsPostBack)
        {
            txtEmail.Text = "demo@legacyauth.local";
            txtPassword.Attributes["value"] = "demo123";
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string email = (txtEmail.Text ?? string.Empty).Trim();
        string password = txtPassword.Text ?? string.Empty;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter email and password.", true);
            return;
        }

        AppUser user = UserStore.Validate(email, password);
        if (user == null)
        {
            ShowMessage("Invalid email or password.", true);
            return;
        }

        Session["UserEmail"] = user.Email;
        Session["UserName"] = user.Name;
        Response.Redirect("Dashboard.aspx");
    }

    private void ShowMessage(string text, bool isError)
    {
        lblMessage.Visible = true;
        lblMessage.Text = text;
        lblMessage.CssClass = isError ? "message error" : "message success";
    }
}
