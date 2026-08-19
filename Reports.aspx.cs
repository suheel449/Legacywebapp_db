using System;
using System.Web.UI;

public partial class ReportsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Reports";
            master.PageHint = "Insights";
        }
    }
}
