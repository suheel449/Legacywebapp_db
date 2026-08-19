using System;
using System.Web.UI;

public partial class DashboardPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Dashboard";
            master.PageHint = "Overview";
        }

        string name = Session["UserName"] as string ?? "User";
        litWelcome.Text = "Welcome back, " + Server.HtmlEncode(name) + ". Here is a snapshot of your workspace.";
    }
}
