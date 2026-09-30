<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="SettingsPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Settings — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Preferences</h2>
    <p class="subtitle">Saved to UserPreferences in MySQL.</p>

    <div class="fields settings-fields">
      <asp:CheckBox ID="chkEmailAlerts" runat="server" Text="Email alerts" />
      <asp:CheckBox ID="chkWeeklyDigest" runat="server" Text="Weekly digest" />
      <asp:CheckBox ID="chkCompactNav" runat="server" Text="Compact navigation" />

      <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
      <asp:Button ID="btnSave" runat="server" Text="Save settings" CssClass="btn primary" OnClick="btnSave_Click" />
    </div>
  </article>
</asp:Content>
