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
            int userId = GetUserId();
            if (userId <= 0)
            {
                return;
            }

            try
            {
                UserPreferences prefs = AppDb.GetPreferences(userId);
                chkEmailAlerts.Checked = prefs.EmailAlerts;
                chkWeeklyDigest.Checked = prefs.WeeklyDigest;
                chkCompactNav.Checked = prefs.CompactNav;
            }
            catch (Exception)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Could not load preferences from the database.";
                lblMessage.CssClass = "message error";
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int userId = GetUserId();
        if (userId <= 0)
        {
            lblMessage.Visible = true;
            lblMessage.Text = "Not signed in.";
            lblMessage.CssClass = "message error";
            return;
        }

        try
        {
            UserPreferences prefs = new UserPreferences
            {
                EmailAlerts = chkEmailAlerts.Checked,
                WeeklyDigest = chkWeeklyDigest.Checked,
                CompactNav = chkCompactNav.Checked
            };
            AppDb.SavePreferences(userId, prefs);
            AppDb.LogActivity(userId, "Settings updated");

            lblMessage.Visible = true;
            lblMessage.Text = "Settings saved to the database.";
            lblMessage.CssClass = "message success";
        }
        catch (Exception)
        {
            lblMessage.Visible = true;
            lblMessage.Text = "Could not save preferences. Check LegacyBizDb.";
            lblMessage.CssClass = "message error";
        }
    }

    private int GetUserId()
    {
        object value = Session["UserId"];
        return value is int ? (int)value : 0;
    }
}
