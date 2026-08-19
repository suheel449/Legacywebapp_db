<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Notifications.aspx.cs" Inherits="NotificationsPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Notifications — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Inbox</h2>
    <p class="subtitle">Alerts for this signed-in session.</p>

    <ul class="notice-list">
      <li class="notice unread">
        <strong>Password policy reminder</strong>
        <span>Use a unique password. This app stores accounts in memory only.</span>
      </li>
      <li class="notice unread">
        <strong>Weekly report is ready</strong>
        <span>Open Reports to review login and registration activity.</span>
      </li>
      <li class="notice">
        <strong>Welcome to Legacy Auth</strong>
        <span>Your dashboard, profile, and settings pages are available after sign-in.</span>
      </li>
      <li class="notice">
        <strong>Session notice</strong>
        <span>Signing out clears this session. Restarting the site also clears users.</span>
      </li>
    </ul>
  </article>
</asp:Content>
