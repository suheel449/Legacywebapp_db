using System;
using System.Web.UI;

public partial class HomePage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserEmail"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        string name = Session["UserName"] as string ?? "User";
        string email = Session["UserEmail"] as string ?? string.Empty;

        litWelcome.Text = "Welcome, " + Server.HtmlEncode(name);
        litName.Text = Server.HtmlEncode(name);
        litEmail.Text = Server.HtmlEncode(email);
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Login.aspx");
    }
}
