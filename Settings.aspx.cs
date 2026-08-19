using System;
using System.Web.UI;

public partial class SettingsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteMaster master = Master as SiteMaster;
        if (master != null)
        {
            master.PageTitle = "Settings";
            master.PageHint = "Preferences";
        }

        if (!IsPostBack)
        {
            chkEmailAlerts.Checked = GetPref("PrefEmailAlerts", true);
            chkWeeklyDigest.Checked = GetPref("PrefWeeklyDigest", true);
            chkCompactNav.Checked = GetPref("PrefCompactNav", false);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Session["PrefEmailAlerts"] = chkEmailAlerts.Checked;
        Session["PrefWeeklyDigest"] = chkWeeklyDigest.Checked;
        Session["PrefCompactNav"] = chkCompactNav.Checked;

        lblMessage.Visible = true;
        lblMessage.Text = "Settings saved for this session.";
        lblMessage.CssClass = "message success";
    }

    private bool GetPref(string key, bool fallback)
    {
        object value = Session[key];
        return value is bool ? (bool)value : fallback;
    }
}
