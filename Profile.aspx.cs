using System;
using System.Web.UI;

public partial class ProfilePage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Profile";
            master.PageHint = "Your account";
        }

        litName.Text = Server.HtmlEncode(Session["UserName"] as string ?? "User");
        litEmail.Text = Server.HtmlEncode(Session["UserEmail"] as string ?? string.Empty);
    }
}
